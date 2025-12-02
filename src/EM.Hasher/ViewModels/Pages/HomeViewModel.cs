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
using CommunityToolkit.WinUI;
using EM.Hasher.Helpers;
using EM.Hasher.Messages;
using EM.Hasher.Messages.UI;
using EM.Hasher.Pages;
using EM.Hasher.Services.Navigation;
using Microsoft.Windows.Storage.Pickers;

namespace EM.Hasher.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public HomeViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        WeakReferenceMessenger.Default.Register<DropFileErrorMessage>(this, (r, m) =>
        {
            if (m is DropFileErrorMessage message)
            {
                DropFileErrorMessage = message.ErrorMessage;
                IsDropFileError = message.IsDropFileError;
            }
        });
    }

    [ObservableProperty]
    public partial string? SelectedFile
    {
        get; private set;
    }

    [ObservableProperty]
    public partial bool IsDropFileError { get; private set; } = false;

    [ObservableProperty]
    public partial string DropFileErrorMessage { get; private set; } = string.Empty;

    public async Task HandleDroppedFile(string droppedFileName)
    {
        WeakReferenceMessenger.Default.Send(
            new DropFileErrorMessage(false, string.Empty));

        await App.MainWindow!.DispatcherQueue.EnqueueAsync(() =>
        {
            SelectNewFile(droppedFileName);
        });
    }

    [RelayCommand]
    private async Task PickFileAsync()
    {
        try
        {
            // Ensure we are running on the UI thread
            await App.MainWindow!.DispatcherQueue.ExtEnqueueAsync(async () =>
            {
                var picker = new FileOpenPicker(App.MainWindow.AppWindow.Id);

                picker.FileTypeFilter.Add("*");
                picker.SuggestedStartLocation = PickerLocationId.Desktop;
                picker.ViewMode = PickerViewMode.List;  // Set view mode to List instead of Thumbnail

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    var selectedFile = file.Path;

                    WeakReferenceMessenger.Default.Send(
                        new DropFileErrorMessage(false, string.Empty));

                    SelectNewFile(selectedFile);
                }
            });
        }
        finally
        {
        }
    }

    private void SelectNewFile(string fileName)
    {
        var itsNew = string.IsNullOrWhiteSpace(SelectedFile) || !fileName.Equals(SelectedFile, StringComparison.InvariantCultureIgnoreCase);
        if (itsNew)
        {
            SelectedFile = fileName;
        }

        // will only trigger re-calculation if a different file is selected
        _navigationService.Navigate<Calculate>(
                        new FilePickedMessage() { FileName = fileName, ItsNew = itsNew });
    }
}
