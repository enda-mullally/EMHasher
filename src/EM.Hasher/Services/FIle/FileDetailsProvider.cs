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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using EM.Hasher.Helpers;
using EM.Hasher.Models;
using Humanizer;

namespace EM.Hasher.Services.File;

public class FileDetailsProvider : IFileDetailsProvider
{
    public async Task<FileDetailsModel?> GetFileDetailsAsync(string fileName)
    {
        return await Task.Run(() =>
        {
            FileInfo? selectedFileInfo = new(fileName);

            if (selectedFileInfo.Exists)
            {
                var (FileVersion, ProductVersion) = GetFileVersionAndFileProductVersion(fileName);

                var unit = selectedFileInfo.Length == 1
                    ? ResourceExtensions.GetLocalized("byte")
                    : ResourceExtensions.GetLocalized("bytes");

                return new FileDetailsModel()
                {
                    FullFileName = fileName,
                    FileCreated = selectedFileInfo.CreationTime.ToString("f") + "  (" + selectedFileInfo.CreationTime.Humanize() + ")",
                    FileModified = selectedFileInfo.LastWriteTime.ToString("f") + "  (" + selectedFileInfo.LastWriteTime.Humanize() + ")",
                    FileName = selectedFileInfo.Name,
                    FileSize = $"{ByteSize.FromBytes(selectedFileInfo.Length).Humanize(format: "#0.00")}  ({selectedFileInfo.Length:N0} {unit})",
                    FileVersion = FileVersion,
                    FileProductVersion = ProductVersion
                };
            }

            return default;
        });
    }

    public static (string FileVersion, string ProductVersion) GetFileVersionAndFileProductVersion(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(fileName))
        {
            return (string.Empty, string.Empty);
        }

        var fileVersionResult = string.Empty;
        var productVersionResult = string.Empty;

        try
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(fileName);

            var fileVersion = versionInfo.FileVersion;
            if (!string.IsNullOrWhiteSpace(fileVersion))
            {
                fileVersionResult = ResourceExtensions.GetLocalized("File") + ": " + fileVersion;
            }

            var productVersion = versionInfo.ProductVersion;
            if (!string.IsNullOrWhiteSpace(productVersion))
            {
                productVersionResult = ResourceExtensions.GetLocalized("Product") + ": " + productVersion;
            }

            return (fileVersionResult, productVersionResult);
        }
        catch
        {
            return (string.Empty, string.Empty);
        }
    }
}
