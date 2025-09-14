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

using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace EM.Hasher.Controls
{
    public sealed partial class HandHoverGrid : Grid
    {
        public HandHoverGrid()
        {
            this.InitializeComponent();
        }

        private InputCursor InputCursor
        {
            get => ProtectedCursor;
            set => ProtectedCursor = value;
        }

        private InputCursor? OriginalInputCursor { get; set; }

        private new void PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            OriginalInputCursor = this.InputCursor ?? InputSystemCursor.Create(InputSystemCursorShape.Arrow);

            this.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
        }

        private new void PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (OriginalInputCursor != null)
            {
                this.InputCursor = OriginalInputCursor;
            }
        }
    }
}
