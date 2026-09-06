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

using System;
using EM.Hasher.DI;
using EM.Hasher.Helpers;
using EM.Hasher.Services;
using EM.Hasher.Services.Activation;
using EM.Hasher.Services.License;
using EM.Hasher.Services.Settings;
using EM.Hasher.ViewModels;
using EM.Hasher.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using WinUIEx;

namespace EM.Hasher;

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

    public static WindowEx? MainWindow
    {
        get; internal set;
    }

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    /// <summary>
    /// Set to true once the Shell's NavigationView has loaded and performed its
    /// default navigation. Until then, any protocol activation must be deferred
    /// (see <see cref="PendingActivationFilePath"/>) so the default Home
    /// navigation does not override it.
    /// </summary>
    public static bool IsShellReady
    {
        get; set;
    }

    /// <summary>
    /// A file supplied via protocol/context-menu activation during a cold start,
    /// to be processed by the Shell once its NavigationView has loaded.
    /// </summary>
    public static string? PendingActivationFilePath
    {
        get; set;
    }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();

        var eventLog = GetService<IEventLogWriter>();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception
        e.Handled = true;
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        try
        {
            var licseService = GetService<ICachedStoreAppLicense>();

            var license =
               await licseService
                .GetCachedStoreAppLicenseAsync();

            var settingsProvider = GetService<ISettingsProvider>();

            MainWindow = GetService<MainWindow>();

            // Set the MainWindow Content.
            if (MainWindow.Content != null)
            {
                var shell = App.GetService<Shell>();

                MainWindow.Content = shell;
            }

            // Apply the saved theme
            var theme = settingsProvider.SelectedTheme switch
            {
                (int)Enums.AppThemeSetting.Dark => ElementTheme.Dark,
                (int)Enums.AppThemeSetting.Light => ElementTheme.Light,
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
            await GetService<IActivationService>().ActivateAsync(MainWindow!);

            // Handle the activation that launched this instance (e.g. a
            // "Hash with EM Hasher" context-menu / emhasher:// protocol launch).
            HandleActivation(AppInstance.GetCurrent().GetActivatedEventArgs());
        }
    }

    private static void HandleActivation(AppActivationArguments args)
    {
        if (args.Kind == ExtendedActivationKind.Protocol &&
            args.Data is IProtocolActivatedEventArgs protocolArgs)
        {
            var filePath = ParseFileFromUri(protocolArgs.Uri);

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (IsShellReady)
                {
                    // The Shell (and its NavigationView) is already loaded, so
                    // route the file through the normal Calculate flow now.
                    _ = GetService<HomeViewModel>().SelectFileAsync(filePath!);
                }
                else
                {
                    // Cold start: defer until the Shell has performed its default
                    // navigation, otherwise it would override us back to Home.
                    PendingActivationFilePath = filePath;
                }
            }
        }
    }

    private static string? ParseFileFromUri(Uri uri)
    {
        try
        {
            // Expected form: emhasher://hash?file=<url-encoded-path>
            var decoder = new WwwFormUrlDecoder(uri.Query);
            return decoder.GetFirstValueByName("file");
        }
        catch
        {
            return null;
        }
    }
}
