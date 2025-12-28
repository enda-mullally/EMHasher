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
using Microsoft.UI.Xaml.Data;

namespace EM.Hasher.Converters
{
    /// <summary>
    /// This converter will convert the internal bool value used to store the casing setting
    /// into the matching UI SelctedIndex value for hash value casing selection.
    /// Note: If the order of the values displayed in the combobox is changed in the UI
    /// don't forget to update here also (including tests).
    /// </summary>
    public partial class CasingSettingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var uiSelectedIndexValue = 0;

            if (value is bool isUpperCase)
            {
                switch (isUpperCase)
                {
                    case true:
                        uiSelectedIndexValue = 0;   // Item 0 in the combobox is "Uppercase"
                        break;

                    default:
                        uiSelectedIndexValue = 1;   // Item 1 in the combobox is "Lowercase"
                        break;
                }
            }

            return uiSelectedIndexValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var boolValue = false;

            if (value is int uiSelectedIndexValue)
            {
                switch (uiSelectedIndexValue)
                {
                    case 0:
                        boolValue = true;   // Item 0 in the combobox is "Uppercase"
                        break;

                    default:
                        boolValue = false;   // Item 1 in the combobox is "Lowercase"
                        break;
                }
            }

            return boolValue;
        }
    }
}
