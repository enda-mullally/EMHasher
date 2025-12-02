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

using Windows.Storage;

namespace EM.Hasher.Services.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly ApplicationDataContainer _localSettings;
        private bool _isTrialMode = false;

        public SettingsProvider()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        public bool IsCrc32Enabled
        {
            set => _localSettings.Values[nameof(IsCrc32Enabled)] = value;
            get => (bool)(_localSettings.Values[nameof(IsCrc32Enabled)] ?? false);
        }

        public bool IsMd5Enabled
        {
            set => _localSettings.Values[nameof(IsMd5Enabled)] = value;
            get => (bool)(_localSettings.Values[nameof(IsMd5Enabled)] ?? true);
        }

        public bool IsSha256Enabled
        {
            set => _localSettings.Values[nameof(IsSha256Enabled)] = value;
            get
            {
                if (_isTrialMode)
                {
                    return false; // In trial mode, SHA-256 is disabled
                }

                return (bool)(_localSettings.Values[nameof(IsSha256Enabled)] ?? true);
            }
        }

        public bool IsSha512Enabled
        {
            set => _localSettings.Values[nameof(IsSha512Enabled)] = value;
            get
            {
                if (_isTrialMode)
                {
                    return false; // In trial mode, SHA-512 is disabled
                }

                return (bool)(_localSettings.Values[nameof(IsSha512Enabled)] ?? false);
            }
        }

        public int SelectedTheme
        {
            set => _localSettings.Values[nameof(SelectedTheme)] = (int)value;
            get => (int)(_localSettings.Values[nameof(SelectedTheme)] ?? 0);
        }

        public bool IsUppercaseHashValues
        {
            set => _localSettings.Values[nameof(IsUppercaseHashValues)] = value;
            get => (bool)(_localSettings.Values[nameof(IsUppercaseHashValues)] ?? false);
        }

        // Note: This property is not persisted; it's set at runtime based on license status.
        // Note: No longer used as the app is now open source/free. Keeping for reference.
        public bool IsTrialMode
        {
            get => _isTrialMode;
            set => _isTrialMode = value;
        }
    }
}
