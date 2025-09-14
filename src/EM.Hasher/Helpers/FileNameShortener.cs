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

namespace EM.Hasher.Helpers
{
    public static class FileNameShortener
    {
        public static string ShortenFilename(string filename, int maxLength)
        {
            if (filename.Length <= maxLength)
            { 
                return filename;
            }
            string extension = Path.GetExtension(filename);
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);

            int keepLength = maxLength - extension.Length - 3; // 3 for "..."
            int firstPartLength = keepLength / 2;
            int lastPartLength = keepLength - firstPartLength;

            string shortened = filenameWithoutExtension.Substring(0, firstPartLength)
                              + "..."
                              + filenameWithoutExtension.Substring(filenameWithoutExtension.Length - lastPartLength)
                              + extension;

            return shortened;
        }
    }
}
