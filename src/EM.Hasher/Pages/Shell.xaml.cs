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

using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Helpers;
using EM.Hasher.Messages.UI;
using EM.Hasher.Services.Navigation;
using EM.Hasher.ViewModels;
using EM.Hasher.ViewModels.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Linq;

namespace EM.Hasher.Pages
{
    public sealed partial class Shell : Page
    {
        private readonly INavigationService _navigationService;

        public UIStateViewModel UiStateViewModel
        {
            get;
        }

        public ShellViewModel ViewModel
        {
            get;
        }

        public Shell(INavigationService navigationService)
        {
            UiStateViewModel = App.GetService<UIStateViewModel>();
            ViewModel = App.GetService<ShellViewModel>();

            App.MainWindow!.SetTitleBar(uxTitleBar);

            this.InitializeComponent();

            uxTitleBar.ActualThemeChanged += UxTitleBar_ActualThemeChanged;
            uxTitleBar.Loaded += UxTitleBar_Loaded;
            _navigationService = navigationService;
            _navigationService.Initialize(contentFrame);

            // I need to use code behind to manually set the binding for the settings item
            // which can't be done via x:bind
            navigationView.LayoutUpdated += this.OnNavigationViewLayoutUpdated!;
        }

        private void OnNavigationViewLayoutUpdated(object sender, object e)
        {
            if (navigationView.SettingsItem is NavigationViewItem settingsItem)
            {
                // Do this only once
                navigationView.LayoutUpdated -= this.OnNavigationViewLayoutUpdated!;

                var binding = new Binding
                {
                    Source = this.UiStateViewModel,
                    Path = new PropertyPath("IsSettingsTabEnabled"),
                    Mode = BindingMode.OneWay
                };

                BindingOperations.SetBinding(settingsItem, NavigationViewItem.IsEnabledProperty, binding);
            }
        }

        private void UxTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            ApplySystemThemeToCaptionButtons((App.MainWindow! as MainWindow)!);
        }

        private void UxTitleBar_ActualThemeChanged(FrameworkElement sender, object args)
        {
            ApplySystemThemeToCaptionButtons((App.MainWindow! as MainWindow)!);
        }

        // TODO
        public void HandleActivation(AppActivationArguments activationArgs)
        {
            //if (activationArgs.Kind == ExtendedActivationKind.Protocol)
            //{
            //    var protocolArgs = activationArgs.Data as ProtocolActivatedEventArgs;
            //    if (protocolArgs != null)
            //    {
            //        string uri = protocolArgs.Uri.AbsoluteUri;
            //        System.Diagnostics.Debug.WriteLine($"Protocol URI handled in MainWindow: {uri}");

            //        // Update the UI or handle the URI logic here
            //        System.Diagnostics.Debug.WriteLine($"Activated with URI: {uri}");

            //        //uxtxtActivated.Text = $"Activated with URI: {uri}";

            //        //--

            //        // Create the Uri object
            //        var protocolUri = new Uri(uri);

            //        // Access the command from the host (e.g., "calculate")
            //        string command = protocolUri.Host; // Extracts the host, which is the command part here

            //        // Access query parameters (file and hash)
            //        var query = protocolUri.Query;
            //        var queryParams = new WwwFormUrlDecoder(query);
            //        string file = queryParams.GetFirstValueByName("file");
            //        string hash = queryParams.GetFirstValueByName("hash");

            //        // Output the values
            //        System.Diagnostics.Debug.WriteLine($"Command: {command}");
            //        System.Diagnostics.Debug.WriteLine($"File: {file}");
            //        System.Diagnostics.Debug.WriteLine($"Hash: {hash}");

            //        // Handle the command (e.g., open the file)
            //        //if (command == "calculate")
            //        {
            //            // Process the 'open' command, use 'file' and 'hash'
            //            //uxtxtParsedQuery.Text = $"calculating file: {file} with hash: {hash}, command: {command}";
            //        }

            //        // uri filename encoding concerns ...

            //        //public string EncodeFilePath(string filePath)
            //        //{
            //        //    // Encode the file path for URL query parameters
            //        //    return Uri.EscapeDataString(filePath);
            //        //}

            //        // .. to encode, needed from Shell launcher...
            //        //string filePath = "d:/my folder/test file.txt";
            //        //string encodedFilePath = Uri.EscapeDataString(filePath);

            //        //Console.WriteLine(encodedFilePath);

            //        // NEEDED HERE, to decode filename path correctly
            //        //string decodedFilePath = Uri.UnescapeDataString(encodedFilePath);
            //        //Console.WriteLine(decodedFilePath); // Outputs: d:/my folder/test file.txt
            //    }
            //}
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                // If we navigate away from Home, clear any FileDrop error messages
                WeakReferenceMessenger.Default.Send(
                    new DropFileErrorMessage(false, string.Empty));

                contentFrame.Navigate(typeof(Pages.Settings));
            }
            else
            {
                var selectedItem = args.SelectedItem as NavigationViewItem;
                if (selectedItem != null)
                {
                    string selectedItemTag = ((string)selectedItem.Tag);
                    sender.Header = selectedItem.Content;

                    // Use a switch statement to avoid reflection
                    Type? pageType = selectedItemTag switch
                    {
                        "Home" => typeof(Pages.Home),
                        "Calculate" => typeof(Pages.Calculate),
                        _ => null
                    };

                    if (pageType != null)
                    {
                        if (pageType != typeof(Home))
                        {
                            // If we navigate away from Home, clear any FileDrop error messages
                            WeakReferenceMessenger.Default.Send(
                                new DropFileErrorMessage(false, string.Empty));
                        }

                        if (pageType == typeof(Calculate))
                        {
                            // If we navigate to the Calculate page, trigger hash calculation for any hash
                            // calculation controls that might now be enabled, which are listening for this
                            // message.
                            WeakReferenceMessenger.Default.Send(
                                new CalculatePageSelectedMessage());
                        }

                        contentFrame.Navigate(pageType);
                    }
                }
            }
            sender.IsBackEnabled = contentFrame.CanGoBack;
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            navigationView.SelectedItem = navigationView.MenuItems[0];
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
        }

        private void ApplySystemThemeToCaptionButtons(Window window)
        {
            var res = Application.Current.Resources;
            Windows.UI.Color buttonForegroundColor;
            Windows.UI.Color buttonHoverForegroundColor;
            Windows.UI.Color buttonHoverBackgroundColor;

            if (this.ActualTheme == ElementTheme.Dark)
            {
                buttonForegroundColor = ColorHelper.GetColorFromHex("#FFFFFF");
                buttonHoverForegroundColor = ColorHelper.GetColorFromHex("#FFFFFF");
                buttonHoverBackgroundColor = ColorHelper.GetColorFromHex("#0FFFFFFF");
            }
            else
            {
                buttonForegroundColor = ColorHelper.GetColorFromHex("#191919");
                buttonHoverForegroundColor = ColorHelper.GetColorFromHex("#191919");
                buttonHoverBackgroundColor = ColorHelper.GetColorFromHex("#09000000");
            }

            res["WindowCaptionForeground"] = buttonForegroundColor;

            window.AppWindow.TitleBar.ButtonForegroundColor = buttonForegroundColor;
            window.AppWindow.TitleBar.ButtonHoverForegroundColor = buttonHoverForegroundColor;
            window.AppWindow.TitleBar.ButtonHoverBackgroundColor = buttonHoverBackgroundColor;
        }

        private void ContentFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            // Ensure correct menu item is selected based on navigation
            foreach (NavigationViewItem item in navigationView.MenuItems.Cast<NavigationViewItem>())
            {
                if (item.Tag as string == e.SourcePageType.Name) // Match by tag
                {
                    navigationView.SelectedItem = item;

                    break;
                }
            }
        }
    }
}
