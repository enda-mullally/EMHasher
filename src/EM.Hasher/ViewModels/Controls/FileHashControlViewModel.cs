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
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Messages;
using EM.Hasher.Messages.UI;
using EM.Hasher.Services.Hashes;
using Windows.ApplicationModel.DataTransfer;

namespace EM.Hasher.ViewModels.Controls;

public partial class FileHashControlViewModel : ObservableObject
{
    private readonly IHashCalculator _hashCalculator;
    private string _fileName = string.Empty;
    private string _hashValue = string.Empty;

    private bool _settingsIsUppercaseHashValues;
    private bool _settingsIsEnabled;

    public FileHashControlViewModel(IHashCalculator hashCalculator, bool isUppercaseHashValues, bool settingsIsEnabled)
    {
        _hashCalculator = hashCalculator;

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

            if (!string.IsNullOrEmpty(_hashValue) && DisplayText!.Equals(_hashValue, System.StringComparison.InvariantCultureIgnoreCase))
            {
                DisplayText = _settingsIsUppercaseHashValues
                    ? _hashValue.ToUpperInvariant()
                    : _hashValue.ToLowerInvariant();
            }
        });

        WeakReferenceMessenger.Default.Register<CalculatePageSelectedMessage>(this, (r, m) =>
        {
            // The Calculate page has been re-selected, so we need to
            // check if we need to start the hash calculation again.
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
    public partial string? AlgorithmName { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? DisplayText { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? ErrorText { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsError { get; private set; } = false;

    [ObservableProperty]
    public partial bool IsTipOpen { get; private set; } = false;

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

            WeakReferenceMessenger.Default.Send(
                new CalculateFileHashStartOrEndMessage(AlgorithmName!, isStart: true));

            await Task.Delay(100);

            _hashValue = string.Empty;
            IsCalculationComplete = false;

            DisplayText = $"Calculating {AlgorithmName} hash...";

            _hashValue = await _hashCalculator.CalculateHashAsync(_fileName);

            DisplayText = _settingsIsUppercaseHashValues
                        ? _hashValue.ToUpperInvariant()
                        : _hashValue.ToLowerInvariant();

            IsCalculationComplete = result = true;

            ShowVirusTotalSearch = AlgorithmName == "SHA-256";
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
                IsTipOpen = false;
            }
        }
        finally
        {
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