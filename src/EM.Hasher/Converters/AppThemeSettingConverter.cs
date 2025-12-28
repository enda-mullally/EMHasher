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
    /// This converter will convert the internal value used to store the app theme setting
    /// into the matching UI SelctedIndex value for the app theme setting combobox.
    /// Note: If the order of the values displayed in the combobox is changed in the UI
    /// don't forget to update here also (including unit tests).
    /// </summary>
    public partial class AppThemeSettingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var uiSelectedIndexValue = 0;   // Item 0 in the combobox is "Default"

            if (value is int settingAppThemeValue)
            {
                switch (settingAppThemeValue)
                {
                    case (int)Enums.AppThemeSetting.Light:
                        uiSelectedIndexValue = 1;   // Item 1 in the comboxob is "Light"
                        break;

                    case (int)Enums.AppThemeSetting.Dark:
                        uiSelectedIndexValue = 2;   // Item 2 in the comboxob is "Dark"
                        break;
                }
            }

            return uiSelectedIndexValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var settingAppThemeValue = 0;

            if (value is int uiSelectedIndexValue)
            {
                switch (uiSelectedIndexValue)
                {
                    // Item 0 in the combobox is "Default"
                    case 0:
                        settingAppThemeValue = (int)Enums.AppThemeSetting.Default;
                        break;

                    // Item 1 in the combobox is "Light"
                    case 1:
                        settingAppThemeValue = (int)Enums.AppThemeSetting.Light;
                        break;

                    // Item 2 in the combobox is "Dark"
                    case 2:
                        settingAppThemeValue = (int)Enums.AppThemeSetting.Dark;
                        break;
                }
            }

            return settingAppThemeValue;
        }
    }
}
