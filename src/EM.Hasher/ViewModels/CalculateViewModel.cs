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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Messages;
using EM.Hasher.Messages.UI;
using EM.Hasher.Services.Authenticode;
using EM.Hasher.Services.Explorer;
using EM.Hasher.Services.File;
using EM.Hasher.Services.Navigation;
using EM.Hasher.Services.Settings;
using EM.Hasher.ViewModels.Controls;
namespace EM.Hasher.ViewModels;

public partial class CalculateViewModel : ObservableObject, INavigationAware
{
    private readonly IFileDetailsProvider _fileDetailsProvider;
    private readonly IAuthenticodeInfoProvider _authenticodeInfoProvider;
    private readonly IExplorerFileSelectorService _explorerFileSelectorService;
    private readonly ISettingsProvider _settingsProvider;
    private string _selectedFileName = string.Empty;

    // Tracks whether the authenticode info has already been loaded for the
    // currently selected file, so toggling the setting off/on does not
    // trigger an unnecessary reload.
    private bool _authenticodeInfoLoaded = false;

    // Small files hash quickly even over a network, so only warn about
    // remote files once they are large enough for transfer latency to matter.
    private const long SlowFilePerformanceThresholdBytes = 100L * 1024 * 1024; // 100 MB

    public ObservableCollection<FileHashControlViewModel> FileHashControlViewModels { get; init; } = [];

    public CalculateViewModel(
        IFileDetailsProvider fileDetailsProvider,
        IAuthenticodeInfoProvider authenticodeInfoProvider,
        IExplorerFileSelectorService explorerFileSelectorService,
        IList<FileHashControlViewModel> fileHashControlViewModels,
        ISettingsProvider settingsProvider)
    {
        _fileDetailsProvider = fileDetailsProvider;
        _authenticodeInfoProvider = authenticodeInfoProvider;
        _explorerFileSelectorService = explorerFileSelectorService;
        _settingsProvider = settingsProvider;

        FileHashControlViewModels = [with(fileHashControlViewModels)];

        ShowAuthenticodeSetting = _settingsProvider.LoadCodeSignCert;

        WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, (r, m) =>
        {
            ShowAuthenticodeSetting = m.LoadCodeSignCert;
        });
    }

    [ObservableProperty]
    public partial bool IsLoadingFileInfo { get; private set; } = false;

    [ObservableProperty]
    public partial string FileLoadingText { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? FileName { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? FileSize { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? FileCreated { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? FileModified { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? FileVersion { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasFileVersion { get; private set; } = false;

    [ObservableProperty]
    public partial string? FileProductVersion { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsLoadingAuthenticodeInfo { get; private set; } = false;

    [ObservableProperty]
    public partial string AuthenticodeLoadingText { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasFileProductVersion { get; private set; } = false;

    [ObservableProperty]
    public partial string? Signer { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? Issuer { get; private set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowAuthenticode))]
    public partial bool IsSigned { get; private set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowAuthenticode))]
    public partial bool ShowAuthenticodeSetting { get; private set; } = false;

    public bool ShowAuthenticode => ShowAuthenticodeSetting && IsSigned;

    [ObservableProperty]
    public partial bool IsTimeStamped { get; private set; } = false;

    [ObservableProperty]
    public partial string? SigningTime { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool ShowSlowFilePerformanceInfoBar { get; private set; } = false;

    private async Task LoadSelectedFileAsync(string selectedFileName, bool itsNew)
    {
        if (!itsNew)
        {
            return;
        }

        _selectedFileName = selectedFileName;
        _authenticodeInfoLoaded = false;

        ShowSlowFilePerformanceInfoBar = ShouldWarnSlowFilePerformance(_selectedFileName);
        WeakReferenceMessenger.Default.Send(
            new HomeFileSelectedMessage(true));

        WeakReferenceMessenger.Default.Send(
            new QueueAllFileHashRequestMessage());

        try
        {
            WeakReferenceMessenger.Default.Send(
                    new IsUiBusyMessage(true));

            await LoadFileInfoAsync();

            await LoadAuthenticodeInfoAsync();

            // A new file is selected, so force recalculation. Fire the hash
            // fan-out last, once the metadata above has rendered.
            WeakReferenceMessenger.Default.Send(
                new CalculateAllFileHashRequestMessage(_selectedFileName, onlyCalculateIfNeeded: false));
        }
        catch (Exception)
        {
            // ignore
        }
    }

    public async Task OnNavigatedToAsync(object parameter)
    {
        if (parameter is FilePickedMessage filePickedMessage)
        {
            await LoadSelectedFileAsync(filePickedMessage.FileName, filePickedMessage.ItsNew);
        }

        await LoadAuthenticodeInfoIfNeededAsync();
    }

    private static bool ShouldWarnSlowFilePerformance(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || IsOnLocalDrive(filePath))
        {
            return false;
        }

        try
        {
            var fileInfo = new FileInfo(filePath);

            // Only warn about remote files large enough for transfer
            // latency to noticeably impact hashing performance.
            return fileInfo.Exists && fileInfo.Length > SlowFilePerformanceThresholdBytes;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsOnLocalDrive(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        try
        {
            var root = Path.GetPathRoot(filePath);

            if (string.IsNullOrWhiteSpace(root))
            {
                return false;
            }

            var driveInfo = new DriveInfo(root);

            return driveInfo.DriveType == DriveType.Fixed;
        }
        catch
        {
            return false;
        }
    }

    private void UpdateAppSubTitle(string appSubTitle)
    {
        WeakReferenceMessenger.Default.Send(
            new SetAppSubTitleMessage(appSubTitle));
    }

    private void SetFileLoadingState(string loadingText)
    {
        FileLoadingText = loadingText;

        // Setting IsSigned here to false to deliberately hide the
        // autheticode laoding ui while we are loading file info details.
        IsSigned = false;
        IsLoadingFileInfo = true;
    }

    private async Task LoadFileInfoAsync()
    {
        SetFileLoadingState(Res.GetLocalized("LoadingFileDetails"));

        var fileDetailsModel = await _fileDetailsProvider.GetFileDetailsAsync(_selectedFileName);

        IsLoadingFileInfo = false; // Hide to loading state (File Details)

        if (fileDetailsModel != null)
        {
            UpdateAppSubTitle($"[{fileDetailsModel.FileName}]");

            FileName = fileDetailsModel.FileName;
            FileSize = fileDetailsModel.FileSize;
            FileCreated = fileDetailsModel.FileCreated;
            FileModified = fileDetailsModel.FileModified;

            FileVersion = fileDetailsModel.FileVersion;
            HasFileVersion = !string.IsNullOrWhiteSpace(FileVersion);

            FileProductVersion = fileDetailsModel.FileProductVersion;
            HasFileProductVersion = !string.IsNullOrWhiteSpace(FileProductVersion);
        }
    }

    private void SetAuthenticodeLoadingState(string loadingText)
    {
        AuthenticodeLoadingText = loadingText;

        // Setting IsSigned here to true to ensure the authenticode
        // loading grid is visible, if the previously selected file
        // was not signed.
        IsLoadingAuthenticodeInfo = IsSigned = true;
    }

    private async Task LoadAuthenticodeInfoIfNeededAsync()
    {
        try
        {
            await LoadAuthenticodeInfoAsync();
        }
        catch (Exception)
        {
            // ignore
        }
    }

    private async Task LoadAuthenticodeInfoAsync()
    {
        if (!ShowAuthenticodeSetting ||
            _authenticodeInfoLoaded ||
            string.IsNullOrWhiteSpace(_selectedFileName))
        {
            return;
        }

        WeakReferenceMessenger.Default.Send(
            new IsUiBusyMessage(true));

        SetAuthenticodeLoadingState(Res.GetLocalized("LoadingAuthenticodeDetails"));
        var signingInfo = await _authenticodeInfoProvider.GetAuthenticodeInfoAsync(_selectedFileName);

        IsLoadingAuthenticodeInfo = false; // Hide to loading state (Authenticode Info)

        _authenticodeInfoLoaded = true;

        if (signingInfo != null)
        {
            IsSigned = signingInfo.IsSigned;
            Signer = signingInfo.Signer;
            Issuer = signingInfo.Issuer;
            IsTimeStamped = signingInfo.IsTimeStamped;
            SigningTime = signingInfo.SigningTime;
        }

        WeakReferenceMessenger.Default.Send(
            new IsUiBusyMessage(false));
    }

    [RelayCommand]
    private async Task OpenFileLocationAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_selectedFileName) || !File.Exists(_selectedFileName))
            {
                // TODO: Show error tip/popup if the file no longer exists 
                return;
            }

            await _explorerFileSelectorService.OpenFileLocationAsync(_selectedFileName);
        }
        finally
        {
        }
    }
}