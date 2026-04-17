/*
 * EM Hasher
 * Copyright © 2026 Enda Mullally (em.apps@outlook.ie)
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

using EM.Hasher.ViewModels.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace EM.Hasher.Views;

public sealed partial class SettingsShell : Page
{
    public UIStateViewModel UiStateViewModel { get; }

    public SettingsShell()
    {
        UiStateViewModel = App.GetService<UIStateViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Always start at the main Settings page when entering SettingsShell
        settingsFrame.Navigate(typeof(Settings));
    }

    private void OnSettingsBreadcrumbTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        // Navigate back to the main Settings page when on a sub-page
        if (UiStateViewModel.CanNavigateBackFromSettingsSubPage && settingsFrame.CanGoBack)
        {
            settingsFrame.GoBack();
        }
    }

    private void SettingsFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        // Update breadcrumb state based on current page
        if (e.SourcePageType == typeof(Settings))
        {
            UiStateViewModel.IsSettingsSubPageVisible = false;
            UiStateViewModel.SettingsSubPageTitle = string.Empty;
        }
        else if (e.SourcePageType == typeof(HashingAlgorithms))
        {
            UiStateViewModel.IsSettingsSubPageVisible = true;
            UiStateViewModel.SettingsSubPageTitle = "Hashing Algorithms";
        }
    }
}
