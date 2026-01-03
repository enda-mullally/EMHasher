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
using EM.Hasher.Helpers;
using EM.Hasher.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace EM.Hasher.Views;

// Now redundant. Look at adding the link to the app store full version on
// the settings page.
public sealed partial class TrialExpired : Page
{
    public ShellViewModel ViewModel
    {
        get;
    }

    public TrialExpired()
    {
        ViewModel = App.GetService<ShellViewModel>();

        InitializeComponent();

        App.MainWindow!.SetTitleBar(uxTitleBarExp);

        uxTitleBarExp.ActualThemeChanged += UxTitleBarExp_ActualThemeChanged;
        uxTitleBarExp.Loaded += UxTitleBarExp_Loaded;
    }

    private void UxTitleBarExp_Loaded(object sender, RoutedEventArgs e)
    {
        ApplySystemThemeToCaptionButtons((App.MainWindow! as MainWindow)!);
    }

    private void UxTitleBarExp_ActualThemeChanged(FrameworkElement sender, object args)
    {
        ApplySystemThemeToCaptionButtons((App.MainWindow! as MainWindow)!);
    }

    public async void HyperlinkButton_ClickAsync(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(
            new Uri("ms-windows-store://pdp/?productid=9NZZHH7X25CG"));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }

    private void ApplySystemThemeToCaptionButtons(Window window)
    {
        var res = Application.Current.Resources;
        Windows.UI.Color buttonForegroundColor;
        Windows.UI.Color buttonHoverForegroundColor;
        Windows.UI.Color buttonHoverBackgroundColor;

        if (ActualTheme == ElementTheme.Dark)
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
}
