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

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Messages;
using EM.Hasher.Messages.UI;
using EM.Hasher.Services.Hashes;
using EM.Hasher.Services.Verification;
using Windows.ApplicationModel.DataTransfer;

namespace EM.Hasher.ViewModels.Controls;

public partial class FileHashControlViewModel : ObservableObject
{
    private readonly IHashCalculator _hashCalculator;
    private readonly IHashVerificationService _hashVerificationService;
    private string _fileName = string.Empty;
    private string _hashValue = string.Empty;

    private bool _settingsIsUppercaseHashValues;
    private bool _settingsIsEnabled;

    public FileHashControlViewModel(
        IHashCalculator hashCalculator,
        IHashVerificationService hashVerificationService,
        bool isUppercaseHashValues,
        bool settingsIsEnabled)
    {
        _hashCalculator = hashCalculator;
        _hashVerificationService = hashVerificationService;

        AlgorithmName = _hashCalculator.GetAlgorithmName();

        WeakReferenceMessenger.Default.Register<CalculateAllFileHashRequestMessage>(this, (r, m) =>
        {
            if (!m.OnlyCalculateIfNeeded)
            {
                // New file is selected, restart hash calculation.
                IsCalculationComplete = ShowVirusTotalSearch = false;
                _fileName = m.FileName;

                _ = StartHashCalculationAsync();
            }
            else
            {
                if (!IsCalculationComplete)
                {
                    _ = StartHashCalculationAsync();
                }
            }
        });

        _settingsIsUppercaseHashValues = isUppercaseHashValues;
        IsEnabled = _settingsIsEnabled = settingsIsEnabled;

        WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, (r, m) =>
        {
            _settingsIsUppercaseHashValues = m.IsUppercaseHashValues;

            IsEnabled = _settingsIsEnabled = m.HashAlgorithmsEnabled[AlgorithmName!];

            if (!string.IsNullOrEmpty(_hashValue) && DisplayText!.Equals(_hashValue, StringComparison.InvariantCultureIgnoreCase))
            {
                DisplayText = _settingsIsUppercaseHashValues
                    ? _hashValue.ToUpperInvariant()
                    : _hashValue.ToLowerInvariant();
            }
        });

        WeakReferenceMessenger.Default.Register<CalculatePageSelectedMessage>(this, (r, m) =>
        {
            // The Calculate page has been re-selected, check
            // if we need to start the hash calculation again.
            if (!string.IsNullOrEmpty(_fileName) &&
                _settingsIsEnabled &&
                !IsCalculationComplete)
            {
                WeakReferenceMessenger.Default.Send(
                    new CalculateAllFileHashRequestMessage(_fileName, true));
            }
        });
    }

    [ObservableProperty]
    public partial bool IsEnabled { get; private set; } = false;

    [ObservableProperty]
    public partial bool IsCalculationComplete { get; private set; } = false;

    [ObservableProperty]
    public partial bool ShowVirusTotalSearch { get; private set; } = false;

    [ObservableProperty]
    public partial bool CalculationInProgress { get; private set; } = false;

    [ObservableProperty]
    public partial int ProgressPercentage { get; private set; } = 0;

    [ObservableProperty]
    public partial string? AlgorithmName { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? DisplayText { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? ErrorText { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsError { get; private set; } = false;

    [ObservableProperty]
    public partial bool IsTipOpen { get; private set; } = false;

    [ObservableProperty]
    public partial bool IsHashVerificationAvailable { get; private set; } = false;

    [ObservableProperty]
    public partial bool IsHashVerified { get; private set; } = false;

    [ObservableProperty]
    public partial string? HashVerificationDescription { get; private set; } = string.Empty;

    private async Task<bool> StartHashCalculationAsync()
    {
        if (!_settingsIsEnabled)
        {
            return false;
        }

        var result = false;

        try
        {
            IsError = false;
            ErrorText = string.Empty;
            CalculationInProgress = true;
            IsHashVerificationAvailable = false;
            ShowVirusTotalSearch = false;

            _hashValue = string.Empty;
            IsCalculationComplete = false;

            DisplayText = $"Calculating {AlgorithmName} hash...";

            ProgressPercentage = 0;

            var progressBaseText = DisplayText;
            var progress = new Progress<int>(percentage =>
            {
                // Ignore late progress callbacks that may arrive after the
                // calculation has completed and the hash value is displayed.
                if (!IsCalculationComplete)
                {
                    ProgressPercentage = percentage;
                    DisplayText = $"{progressBaseText}  ({percentage}%)";
                }
            });

            WeakReferenceMessenger.Default.Send(
                new CalculateFileHashStartOrEndMessage(AlgorithmName!, isStart: true));

            // Run the hash calculation on a background thread. The read/hash loop
            // often completes synchronously (e.g. when the file is served from the
            // OS cache, which is common when several algorithms hash the same file),
            // which would otherwise run inline on the UI thread and starve the
            // dispatcher so the queued Progress<int> callbacks never render. The
            // Progress<int> above captured the UI SynchronizationContext, so its
            // callbacks still marshal back to the UI thread for real-time updates.
            _hashValue = await Task.Run(() => _hashCalculator.CalculateHashAsync(_fileName, progress));

            DisplayText = _settingsIsUppercaseHashValues
                        ? _hashValue.ToUpperInvariant()
                        : _hashValue.ToLowerInvariant();

            IsCalculationComplete = result = true;

            ShowVirusTotalSearch = AlgorithmName == "SHA-256";

            var verification = await _hashVerificationService.VerifyAsync(_fileName, _hashValue);

            IsHashVerificationAvailable = verification.VerificationHashFound;
            IsHashVerified = verification.IsHashMatching;
            HashVerificationDescription = verification.HashVerificationDescription;
        }
        catch (Exception ex)
        {
            ErrorText = $"Error: {ex.Message}";
            IsError = true;
        }
        finally
        {
            CalculationInProgress = false;

            WeakReferenceMessenger.Default.Send(
                new CalculateFileHashStartOrEndMessage(AlgorithmName!, isStart: false));
        }

        return result;
    }

    [RelayCommand]
    private async Task CopyHash()
    {
        try
        {
            // Copy the hash value in DisplayText to the clipboard
            if (string.IsNullOrEmpty(DisplayText))
            {
                return;
            }

            if (IsCalculationComplete && !IsError)
            {
                DataPackage hashValuePackage = new();
                hashValuePackage.SetText(DisplayText!);
                Clipboard.SetContent(hashValuePackage);

                IsTipOpen = true;
                await Task.Delay(2000); // Display for 2 seconds
            }
        }
        finally
        {
            IsTipOpen = false;
        }
    }

    [RelayCommand]
    private async Task SearchHash()
    {
        try
        {
            // Open the default web browser and search the hash value on VirusTotal
            if (string.IsNullOrEmpty(DisplayText))
            {
                return;
            }

            if (IsCalculationComplete && !IsError)
            {
                var virusTotalUrl = $"https://www.virustotal.com/gui/search/{DisplayText}";
                var uri = new Uri(virusTotalUrl);
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }
        finally
        {
        }
    }
}