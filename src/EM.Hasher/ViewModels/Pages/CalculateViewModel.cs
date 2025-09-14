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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Messages;
using EM.Hasher.Messages.UI;
using EM.Hasher.Services.File;
using EM.Hasher.Services.Navigation;
using EM.Hasher.ViewModels.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EM.Hasher.ViewModels.Pages;

public partial class CalculateViewModel : ObservableObject, INavigationAware
{
    private readonly IFileDetailsProvider _fileDetailsProvider;
    public ObservableCollection<FileHashControlViewModel> FileHashControlViewModels { get; } = [];

    public CalculateViewModel(
        IFileDetailsProvider fileDetailsProvider,
        IList<FileHashControlViewModel> fileHashControlViewModels)
    {
        _fileDetailsProvider = fileDetailsProvider;

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

            WeakReferenceMessenger.Default.Send(
                new HomeFileSelectedMessage(true));

            WeakReferenceMessenger.Default.Send(
                new CalculateAllFileHashRequestMessage(selectedFileName, false));

            var fileDetailsModel = await
                _fileDetailsProvider.GetFileDetailsAsync(selectedFileName);

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
}