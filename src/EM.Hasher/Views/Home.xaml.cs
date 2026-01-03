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
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Controls;
using EM.Hasher.Messages.UI;
using EM.Hasher.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace EM.Hasher.Views;

public sealed partial class Home : Page
{
    public HomeViewModel ViewModel
    {
        get;
    }

    public Home()
    {
        ViewModel = App.GetService<HomeViewModel>();

        InitializeComponent();
    }

    private async void HandleDragOverAsync(DragEventArgs e, DragOperationDeferral deferral)
    {
        try
        {
            if (e == null || e.DataView == null || deferral == null)
            {
                return;
            }

            var items = await e.DataView.GetStorageItemsAsync();

            if (items != null && items.Count == 1 && items[0] is StorageFile)
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                IsDropGreen = true;

                WeakReferenceMessenger.Default.Send(
                    new DropFileErrorMessage(false, string.Empty));
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
                IsDropGreen = false;

                WeakReferenceMessenger.Default.Send(
                    new DropFileErrorMessage(true, "One at a time please :-)"));
            }
        }
        finally
        {
            // Complete the deferral to indicate that the drag operation is finished.
            deferral?.Complete();
        }
    }

    private void uxDropFileControl_DragOver(object sender, DragEventArgs e)
    {
        if (sender == null || e == null)
        {
            return;
        }

        e.AcceptedOperation = DataPackageOperation.Copy;

        // Hide default drag visuals
        e.DragUIOverride.IsGlyphVisible = false;
        e.DragUIOverride.Caption = "";                     // <-- Clear the caption
        e.DragUIOverride.IsCaptionVisible = false;
        e.Handled = true;

        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var deferral = e.GetDeferral();

            HandleDragOverAsync(e, deferral);
        }
        else
        {
            e.AcceptedOperation = DataPackageOperation.None;

            IsDropGreen = false;
        }
    }

    private async void uxDropFileControl_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var deferral = e.GetDeferral();
            try
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count == 1 && items[0] is StorageFile file)
                {
                    if (File.Exists(file.Path)) // ensure it's a classic file and not a link or shortcut from the start menu etc.
                    {
                        IsDropGreen = false;

                        await ViewModel.HandleDroppedFile(file.Path);
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(
                            new DropFileErrorMessage(true, "Please drop a valid file! Special links are not supported."));
                    }
                }
            }
            finally
            {
                deferral.Complete();
            }
        }
    }

    private void uxDropFileControl_DragLeave(object sender, DragEventArgs e)
    {
        IsDropGreen = false;
    }

    public bool IsDropGreen
    {
        get => (bool)GetValue(IsDropGreenProperty);
        set => SetValue(IsDropGreenProperty, value);
    }

    public static readonly DependencyProperty IsDropGreenProperty =
        DependencyProperty.Register(nameof(IsDropGreen), typeof(bool), typeof(DropFileControl), new PropertyMetadata(false));
}
