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
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Messages.UI;
using EM.Hasher.Services.Application;
using EM.Hasher.Services.License;
using EM.Hasher.Services.Settings;
using Microsoft.UI.Xaml;
using WinUIEx;

namespace EM.Hasher.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ICachedStoreAppLicense _cachedStoreAppLicense;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IAppVersion _appVersion;
    private readonly WindowEx _currentWindow = null!;
    private Dictionary<string, bool> _hashAlgorithmsEnabled = [];
    private bool _initialized = false;

    [ObservableProperty]
    public partial string? VersionDescription
    {
        get; private set;
    }

    [ObservableProperty]
    public partial string? TrialLicenseDescription
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

    public bool IsAlgorithmSelectionInvalid => !IsCrc32Enabled && !IsMd5Enabled && !IsSha256Enabled && !IsSha512Enabled;

    public bool IsTrialMode => _settingsProvider.IsTrialMode;

    public SettingsViewModel(
        ICachedStoreAppLicense cachedStoreAppLicenseModel,
        ISettingsProvider settingsProvider,
        IAppVersion appVersion)
    {
        _cachedStoreAppLicense = cachedStoreAppLicenseModel;
        _settingsProvider = settingsProvider;
        _appVersion = appVersion;

        _hashAlgorithmsEnabled["CRC-32"] = _settingsProvider.IsCrc32Enabled;
        _hashAlgorithmsEnabled["MD5"] = _settingsProvider.IsMd5Enabled;
        _hashAlgorithmsEnabled["SHA-256"] = _settingsProvider.IsSha256Enabled;
        _hashAlgorithmsEnabled["SHA-512"] = settingsProvider.IsSha512Enabled;

        // Init observables
        IsCrc32Enabled = _settingsProvider.IsCrc32Enabled;
        IsMd5Enabled = _settingsProvider.IsMd5Enabled;
        IsSha256Enabled = _settingsProvider.IsSha256Enabled;
        IsSha512Enabled = _settingsProvider.IsSha512Enabled;
        IsUppercaseHashValues = _settingsProvider.IsUppercaseHashValues;
        ThemeSelectedIndex = _settingsProvider.SelectedTheme;

        _currentWindow = App.MainWindow!;

        VersionDescription = _appVersion.GetVersionDescription();

        _ = GetTrialLicenseDescriptionAsync();

        _initialized = true;
    }

    [ObservableProperty]
    public partial int ThemeSelectedIndex
    {
        get; set;
    }

    private async Task GetTrialLicenseDescriptionAsync()
    {
        var licenseModel = await _cachedStoreAppLicense.GetCachedStoreAppLicenseAsync();

        if (licenseModel != null)
        {
            if (licenseModel.IsActive && licenseModel.IsTrial)
            {
                TrialLicenseDescription = $"{Environment.NewLine}{Environment.NewLine}Trial Version: Some features are disabled.";
            }
        }
        else
        {
            // I've seen this happen on accounts which have just redeemed a code to download the app,
            // don't worry, it's licensed, as windows handles this, but the store api hasn't updated the license yet
            TrialLicenseDescription = string.Empty;
        }
    }

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

    partial void OnIsCrc32EnabledChanged(bool value)
    {
        OnAlgorithmEnabled();
    }

    partial void OnIsMd5EnabledChanged(bool value)
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

    partial void OnThemeSelectedIndexChanged(int value)
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

        if (_settingsProvider.SelectedTheme != value)
        {
            _settingsProvider.SelectedTheme = value;

            // Apply the new theme
            if (_currentWindow is null || _currentWindow.Content is not FrameworkElement)
            {
                return;
            }

            ((FrameworkElement)_currentWindow.Content).RequestedTheme = value switch
            {
                1 => ElementTheme.Dark,
                2 => ElementTheme.Light,
                _ => ElementTheme.Default
            };
        }
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

        _hashAlgorithmsEnabled["CRC-32"] = _settingsProvider.IsCrc32Enabled = IsCrc32Enabled;
        _hashAlgorithmsEnabled["MD5"] = _settingsProvider.IsMd5Enabled = IsMd5Enabled;
        _hashAlgorithmsEnabled["SHA-256"] = _settingsProvider.IsSha256Enabled = IsSha256Enabled;
        _hashAlgorithmsEnabled["SHA-512"] = _settingsProvider.IsSha512Enabled = IsSha512Enabled;

        WeakReferenceMessenger.Default.Send(
           new SettingsChangedMessage(_hashAlgorithmsEnabled, IsUppercaseHashValues));
    }
}
