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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Messages;
using EM.Hasher.Messages.UI;
using EM.Hasher.Services.Explorer;
using EM.Hasher.Services.File;
using EM.Hasher.Services.Navigation;
using EM.Hasher.ViewModels.Controls;

namespace EM.Hasher.ViewModels;

public partial class CalculateViewModel : ObservableObject, INavigationAware
{
    private readonly IFileDetailsProvider _fileDetailsProvider;
    private readonly IExplorerFileSelectorService _explorerFileSelectorService;
    private string _selectedFileName = string.Empty;

    public ObservableCollection<FileHashControlViewModel> FileHashControlViewModels { get; init; } = [];

    public CalculateViewModel(
        IFileDetailsProvider fileDetailsProvider,
        IExplorerFileSelectorService explorerFileSelectorService,
        IList<FileHashControlViewModel> fileHashControlViewModels)
    {
        _fileDetailsProvider = fileDetailsProvider;
        _explorerFileSelectorService = explorerFileSelectorService;

        FileHashControlViewModels = new ObservableCollection<FileHashControlViewModel>(fileHashControlViewModels);
    }

    [ObservableProperty]
    public partial string? FileName { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? FileSize { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? FileCreated { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string? FileModified { get; private set; } = string.Empty;

    private async Task LoadSelectedFileAsync(string selectedFileName, bool itsNew)
    {
        try
        {
            if (!itsNew)
            {
                return;
            }

            _selectedFileName = selectedFileName;

            WeakReferenceMessenger.Default.Send(
                new HomeFileSelectedMessage(true));

            WeakReferenceMessenger.Default.Send(
                new CalculateAllFileHashRequestMessage(_selectedFileName, onlyCalculateIfNeeded: false)); // a new file is selected, so force recalculation

            var fileDetailsModel = await
                _fileDetailsProvider.GetFileDetailsAsync(_selectedFileName);

            if (fileDetailsModel != null)
            {
                UpdateAppSubTitle($"[{fileDetailsModel.FileName}]");

                FileName = fileDetailsModel.FileName;
                FileSize = fileDetailsModel.FileSize;
                FileCreated = fileDetailsModel.FileCreated;
                FileModified = fileDetailsModel.FileModified;
            }
        }
        finally
        {
        }
    }

    public async Task OnNavigatedToAsync(object parameter)
    {
        if (parameter == null)
        {
            return;
        }

        if (parameter is FilePickedMessage filePickedMessage)
        {
            await LoadSelectedFileAsync(filePickedMessage.FileName, filePickedMessage.ItsNew);
        }
    }

    public void UpdateAppSubTitle(string appSubTitle)
    {
        WeakReferenceMessenger.Default.Send(
            new SetAppSubTitleMessage(appSubTitle));
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