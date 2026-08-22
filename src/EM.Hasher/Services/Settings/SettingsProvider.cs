/*
 * EM Hasher
 * Copyright © 2025-2026 Enda Mullally (em.apps@outlook.ie)
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

namespace EM.Hasher.Services.Settings;

public class SettingsProvider : ISettingsProvider
{
    private readonly ApplicationDataContainer _localSettings;

    public SettingsProvider()
    {
        _localSettings = ApplicationData.Current.LocalSettings;
    }

    public bool IsBlake3_Enabled
    {
        set => _localSettings.Values[nameof(IsBlake3_Enabled)] = value;
        get => (bool)(_localSettings.Values[nameof(IsBlake3_Enabled)] ?? false);
    }

    public bool IsCrc32_Enabled
    {
        set => _localSettings.Values[nameof(IsCrc32_Enabled)] = value;
        get => (bool)(_localSettings.Values[nameof(IsCrc32_Enabled)] ?? false);
    }

    public bool IsMd5_Enabled
    {
        set => _localSettings.Values[nameof(IsMd5_Enabled)] = value;
        get => (bool)(_localSettings.Values[nameof(IsMd5_Enabled)] ?? true);
    }

    public bool IsSha1_Enabled
    {
        set => _localSettings.Values[nameof(IsSha1_Enabled)] = value;
        get => (bool)(_localSettings.Values[nameof(IsSha1_Enabled)] ?? false);
    }

    public bool IsSha256_Enabled
    {
        set => _localSettings.Values[nameof(IsSha256_Enabled)] = value;
        get => (bool)(_localSettings.Values[nameof(IsSha256_Enabled)] ?? false);
    }

    public bool IsSha512_Enabled
    {
        set => _localSettings.Values[nameof(IsSha512_Enabled)] = value;
        get => (bool)(_localSettings.Values[nameof(IsSha512_Enabled)] ?? false);
    }

    public bool IsSha3_256_Enabled
    {
        set => _localSettings.Values[nameof(IsSha3_256_Enabled)] = value;
        get => (bool)(_localSettings.Values[nameof(IsSha3_256_Enabled)] ?? false);
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

    public bool IsNavigationPaneOpen
    {
        set => _localSettings.Values[nameof(IsNavigationPaneOpen)] = value;
        get => (bool)(_localSettings.Values[nameof(IsNavigationPaneOpen)] ?? true);
    }
}
