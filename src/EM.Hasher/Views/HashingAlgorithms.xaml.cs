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

using EM.Hasher.Services.Navigation;
using EM.Hasher.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace EM.Hasher.Views;

public sealed partial class HashingAlgorithms : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public HashingAlgorithms()
    {
        ViewModel = App.GetService<SettingsViewModel>();

        InitializeComponent();
    }

    private void OnBreadcrumbBarItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (args.Index == 0)
        {
            // Only allow navigating back if the algorithm selection is valid.
            if (!ViewModel.IsAlgorithmSelectionInvalid)
            {
                var navigationService = App.GetService<INavigationService>();

                navigationService.GoBack();
            }
        }
    }
}
