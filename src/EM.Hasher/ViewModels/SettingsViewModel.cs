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

using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Helpers;
using EM.Hasher.Messages.UI;
using EM.Hasher.Services.Application;
using EM.Hasher.Services.Hashes;
using EM.Hasher.Services.License;
using EM.Hasher.Services.Settings;
using Microsoft.UI.Xaml;
using WinUIEx;

namespace EM.Hasher.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IAppVersion _appVersion;
    private readonly WindowEx _currentWindow = null!;
    private readonly Dictionary<string, bool> _hashAlgorithmsEnabled = [];
    private readonly bool _initialized = false;

    [ObservableProperty]
    public partial string? VersionDescription
    {
        get; private set;
    }

    [ObservableProperty]
    public partial bool IsUppercaseHashValues
    {
        get; set;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgorithmSelectionInvalid))]
    public partial bool IsBlake3Enabled
    {
        get; set;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgorithmSelectionInvalid))]
    public partial bool IsCrc32Enabled
    {
        get; set;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgorithmSelectionInvalid))]
    public partial bool IsMd5Enabled
    {
        get; set;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgorithmSelectionInvalid))]
    public partial bool IsSha1Enabled
    {
        get; set;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgorithmSelectionInvalid))]
    public partial bool IsSha256Enabled
    {
        get; set;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgorithmSelectionInvalid))]
    public partial bool IsSha512Enabled
    {
        get; set;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgorithmSelectionInvalid))]
    public partial bool IsSha3_256Enabled
    {
        get; set;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgorithmSelectionInvalid))]
    public partial bool IsSha3_512Enabled
    {
        get; set;
    }

    public bool IsAlgorithmSelectionInvalid =>
        !IsBlake3Enabled &&
        !IsCrc32Enabled &&
        !IsMd5Enabled &&
        !IsSha1Enabled &&
        !IsSha256Enabled &&
        !IsSha512Enabled &&
        !IsSha3_256Enabled &&
        !IsSha3_512Enabled;

    public SettingsViewModel(
        ICachedStoreAppLicense cachedStoreAppLicenseModel,
        ISettingsProvider settingsProvider,
        IAppVersion appVersion)
    {
        _settingsProvider = settingsProvider;
        _appVersion = appVersion;

        _hashAlgorithmsEnabled["BLAKE3"] = _settingsProvider.IsBlake3_Enabled;
        _hashAlgorithmsEnabled["CRC-32"] = _settingsProvider.IsCrc32_Enabled;
        _hashAlgorithmsEnabled["MD5"] = _settingsProvider.IsMd5_Enabled;
        _hashAlgorithmsEnabled["SHA-1"] = _settingsProvider.IsSha1_Enabled;
        _hashAlgorithmsEnabled["SHA-256"] = _settingsProvider.IsSha256_Enabled;
        _hashAlgorithmsEnabled["SHA-512"] = _settingsProvider.IsSha512_Enabled;
        _hashAlgorithmsEnabled["SHA3-256"] = Sha3_256HashCalculator.IsAvailable
            && _settingsProvider.IsSha3_256_Enabled;
        _hashAlgorithmsEnabled["SHA3-512"] = Sha3_512HashCalculator.IsAvailable
            && _settingsProvider.IsSha3_512_Enabled;

        // Init observables
        IsBlake3Enabled = _settingsProvider.IsBlake3_Enabled;
        IsCrc32Enabled = _settingsProvider.IsCrc32_Enabled;
        IsMd5Enabled = _settingsProvider.IsMd5_Enabled;
        IsSha1Enabled = _settingsProvider.IsSha1_Enabled;
        IsSha256Enabled = _settingsProvider.IsSha256_Enabled;
        IsSha512Enabled = _settingsProvider.IsSha512_Enabled;
        IsSha3_256Enabled = Sha3_256HashCalculator.IsAvailable
            ? _settingsProvider.IsSha3_256_Enabled
            : false;
        IsSha3_512Enabled = Sha3_512HashCalculator.IsAvailable
            ? _settingsProvider.IsSha3_512_Enabled
            : false;
        IsUppercaseHashValues = _settingsProvider.IsUppercaseHashValues;
        ThemeSelectedIndex = _settingsProvider.SelectedTheme;

        _currentWindow = App.MainWindow!;
        VersionDescription = _appVersion.GetVersionDescription();
        _initialized = true;
    }

    [ObservableProperty]
    public partial int ThemeSelectedIndex
    {
        get; set;
    }

    public bool IsSha3_256_Available => Sha3_256HashCalculator.IsAvailable;

    public bool IsSha3_512_Available => Sha3_512HashCalculator.IsAvailable;

    partial void OnIsUppercaseHashValuesChanged(bool value)
    {
        if (!_initialized)
        {
            // We don't want to send messages until the ViewModel is fully initialized
            return;
        }

        WeakReferenceMessenger.Default.Send(
            new SettingsSelectionMessage(!IsAlgorithmSelectionInvalid));

        if (IsAlgorithmSelectionInvalid)
        {
            return;
        }

        WeakReferenceMessenger.Default.Send(
          new SettingsChangedMessage(_hashAlgorithmsEnabled, value));

        _settingsProvider.IsUppercaseHashValues = value;
    }

    partial void OnIsBlake3EnabledChanged(bool value)
    {
        OnAlgorithmEnabled();
    }

    partial void OnIsCrc32EnabledChanged(bool value)
    {
        OnAlgorithmEnabled();
    }

    partial void OnIsMd5EnabledChanged(bool value)
    {
        OnAlgorithmEnabled();
    }

    partial void OnIsSha1EnabledChanged(bool value)
    {
        OnAlgorithmEnabled();
    }

    partial void OnIsSha256EnabledChanged(bool value)
    {
        OnAlgorithmEnabled();
    }

    partial void OnIsSha512EnabledChanged(bool value)
    {
        OnAlgorithmEnabled();
    }

    partial void OnIsSha3_256EnabledChanged(bool value)
    {
        OnAlgorithmEnabled();
    }

    partial void OnIsSha3_512EnabledChanged(bool value)
    {
        OnAlgorithmEnabled();
    }

    partial void OnThemeSelectedIndexChanged(int value)
    {
        if (!_initialized)
        {
            // We don't want to send messages until the ViewModel is fully initialized
            return;
        }

        WeakReferenceMessenger.Default.Send(
            new SettingsSelectionMessage(!IsAlgorithmSelectionInvalid));

        if (_settingsProvider.SelectedTheme == value)
        {
            return;
        }

        _settingsProvider.SelectedTheme = value;

        if (_currentWindow?.Content is not FrameworkElement content)
        {
            return;
        }

        var theme = value switch
        {
            1 => ElementTheme.Dark,
            2 => ElementTheme.Light,
            _ => ElementTheme.Default
        };

        content.RequestedTheme = theme;
        TitleBarHelper.ApplySystemThemeToCaptionButtons(theme);
    }

    private void OnAlgorithmEnabled()
    {
        if (!_initialized)
        {
            // We don't want to send messages until the ViewModel is fully initialized
            return;
        }

        WeakReferenceMessenger.Default.Send(
            new SettingsSelectionMessage(!IsAlgorithmSelectionInvalid));

        if (IsAlgorithmSelectionInvalid)
        {
            return;
        }

        _hashAlgorithmsEnabled["BLAKE3"] = _settingsProvider.IsBlake3_Enabled = IsBlake3Enabled;
        _hashAlgorithmsEnabled["CRC-32"] = _settingsProvider.IsCrc32_Enabled = IsCrc32Enabled;
        _hashAlgorithmsEnabled["MD5"] = _settingsProvider.IsMd5_Enabled = IsMd5Enabled;
        _hashAlgorithmsEnabled["SHA-1"] = _settingsProvider.IsSha1_Enabled = IsSha1Enabled;
        _hashAlgorithmsEnabled["SHA-256"] = _settingsProvider.IsSha256_Enabled = IsSha256Enabled;
        _hashAlgorithmsEnabled["SHA-512"] = _settingsProvider.IsSha512_Enabled = IsSha512Enabled;
        _hashAlgorithmsEnabled["SHA3-256"] =
            Sha3_256HashCalculator.IsAvailable && (_settingsProvider.IsSha3_256_Enabled = IsSha3_256Enabled);
        _hashAlgorithmsEnabled["SHA3-512"] =
            Sha3_512HashCalculator.IsAvailable && (_settingsProvider.IsSha3_512_Enabled = IsSha3_512Enabled);
        
        WeakReferenceMessenger.Default.Send(
           new SettingsChangedMessage(_hashAlgorithmsEnabled, IsUppercaseHashValues));
    }
}
