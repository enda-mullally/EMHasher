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

using EM.Hasher.ViewModels.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EM.Hasher.Controls
{
    public sealed partial class FileHashControl : UserControl
    {
        public FileHashControlViewModel ViewModel
        {
            get => (FileHashControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(FileHashControlViewModel),
                typeof(FileHashControlViewModel),
                typeof(FileHashControl),
                new PropertyMetadata(null));

        public FileHashControl()
        {
            this.InitializeComponent();
        }
    }
}
