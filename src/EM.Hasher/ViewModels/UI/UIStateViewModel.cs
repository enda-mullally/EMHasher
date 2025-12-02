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

using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Messages;
using EM.Hasher.Messages.UI;
using Microsoft.UI.Xaml.Data;

namespace EM.Hasher.ViewModels.UI
{
    [Bindable]
    public partial class UIStateViewModel : ObservableObject
    {
        private readonly Dictionary<string, bool> _calculationNameInProgress = [];

        public UIStateViewModel()
        {
            // Register for messages
            WeakReferenceMessenger.Default.Register<SettingsSelectionMessage>(this, (r, m) =>
            {
                SettingsSelectionIsValid = m.IsSettingsSelectionValid;
            });

            WeakReferenceMessenger.Default.Register<HomeFileSelectedMessage>(this, (r, m) =>
            {
                HomeFileIsSelected = m.IsFileSelected;
            });

            WeakReferenceMessenger.Default.Register<CalculateFileHashStartOrEndMessage>(this, (r, m) =>
            {
                _calculationNameInProgress[m.AlgorithmName] = m.IsStart;

                IsUiBusy = _calculationNameInProgress.Any(c => c.Value == true);
            });
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsHomeTabEnabled))]
        [NotifyPropertyChangedFor(nameof(IsCalculateTabEnabled))]
        [NotifyPropertyChangedFor(nameof(IsSettingsTabEnabled))]
        private partial bool SettingsSelectionIsValid { get; set; } = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsHomeTabEnabled))]
        [NotifyPropertyChangedFor(nameof(IsCalculateTabEnabled))]
        [NotifyPropertyChangedFor(nameof(IsSettingsTabEnabled))]
        private partial bool HomeFileIsSelected { get; set; } = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsHomeTabEnabled))]
        [NotifyPropertyChangedFor(nameof(IsCalculateTabEnabled))]
        [NotifyPropertyChangedFor(nameof(IsSettingsTabEnabled))]
        private partial bool IsUiBusy { get; set; } = false;

        // These properties are used to enable/disable the tabs in the navigation view, they
        // will be bound to the UI via x:bind.

        public bool IsHomeTabEnabled => !IsUiBusy && SettingsSelectionIsValid;

        public bool IsCalculateTabEnabled => SettingsSelectionIsValid && HomeFileIsSelected;

        public bool IsSettingsTabEnabled => !IsUiBusy;
    }
}
