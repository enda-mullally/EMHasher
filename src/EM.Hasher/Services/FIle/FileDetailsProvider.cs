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

using System.IO;
using System.Threading.Tasks;
using EM.Hasher.Models;
using Humanizer;

namespace EM.Hasher.Services.File
{
    public class FileDetailsProvider : IFileDetailsProvider
    {
        public async Task<FileDetailsModel?> GetFileDetailsAsync(string fileName)
        {
            return await Task.Run(() =>
            {
                FileInfo? selectedFileInfo = new(fileName);

                if (selectedFileInfo.Exists)
                {
                    return new FileDetailsModel()
                    {
                        FullFileName = fileName,
                        FileCreated = selectedFileInfo.CreationTime.ToString("f") + " (" + selectedFileInfo.CreationTime.Humanize() + ")",
                        FileModified = selectedFileInfo.LastWriteTime.ToString("f") + " (" + selectedFileInfo.LastWriteTime.Humanize() + ")",
                        FileName = selectedFileInfo.Name,
                        FileSize = ByteSize.FromBytes(selectedFileInfo.Length).Humanize(format: "#0.00")
                    };
                }

                return default;
            });
        }
    }
}
