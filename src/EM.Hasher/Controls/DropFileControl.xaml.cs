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

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;

namespace EM.Hasher.Controls
{
    public sealed partial class DropFileControl : UserControl
    {
        public DropFileControl()
        {
            InitializeComponent();
        }

        public Color DashColor
        {
            get => (Color)GetValue(DashColorProperty);
            set => SetValue(DashColorProperty, value);
        }

        public static readonly DependencyProperty DashColorProperty =
            DependencyProperty.Register(nameof(DashColor), typeof(Color), typeof(DropFileControl), new PropertyMetadata(Colors.Gray));
    }
}
