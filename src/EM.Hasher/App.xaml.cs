/*
 * EM Hasher
 * Copyright © 2025 Enda Mullally (em.apps@outlook.ie)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using EM.Hasher.DI;
using EM.Hasher.Helpers;
using EM.Hasher.Pages;
using EM.Hasher.Services;
using EM.Hasher.Services.License;
using EM.Hasher.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using WinUIEx;

namespace EM.Hasher
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private static readonly IServiceProvider ServiceProvider =
            Container
                .Create()
                .BuildServiceProvider();

        public static T GetService<T>() where T : class
        {
            var service =
                ServiceProvider.GetService<T>() ?? throw
                        new NullReferenceException("Could not create type [" + typeof(T) + "]. Please ensure this type is registered.");

            return service;
        }

        public static WindowEx? MainWindow { get; private set; }

        public static UIElement? AppTitlebar { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            var eventLog = GetService<IEventLogWriter>();
            
            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO: Log and handle exceptions as appropriate.
            // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                var licseService = GetService<ICachedStoreAppLicense>();

                var license =
                   await licseService
                    .GetCachedStoreAppLicenseAsync();

                var settingsProvider = GetService<ISettingsProvider>();

                if (MainWindow == null)
                {
                    MainWindow = new MainWindow();
                }

                // Set the MainWindow Content.
                if (MainWindow.Content != null)
                {
                    if (!license!.IsActive || (license!.IsTrial))
                    {
                        settingsProvider.IsTrialMode = true;
                    }
                    
                    var shell = App.GetService<Shell>();

                    MainWindow.Content = shell;
                }                

                // Apply the saved theme
                var theme = settingsProvider.SelectedTheme switch
                {
                    1 => ElementTheme.Dark,
                    2 => ElementTheme.Light,
                    _ => ElementTheme.Default
                };

                ((FrameworkElement)MainWindow.Content!).RequestedTheme = theme;
                TitleBarHelper.ApplySystemThemeToCaptionButtons(theme);
            }
            catch
            {
            }
            finally
            {
                MainWindow!.Activate();
            }
        }
    }
}
