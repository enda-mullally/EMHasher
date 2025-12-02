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
using System.Threading.Tasks;
using Force.Crc32;

namespace EM.Hasher.Services.Hashes
{
    public class Crc32HashCalculator : IHashCalculator
    {
        public async Task<string> CalculateHashAsync(string fileName)
        {
            using var crc32 = new Crc32Algorithm();

            await using var fileStream = new FileStream(fileName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                IHashCalculator.BufferSize,
                useAsync: true);

            using var bufferedStream = new BufferedStream(fileStream, IHashCalculator.BufferSize);

            var hashBytes = await crc32.ComputeHashAsync(bufferedStream);

            return Convert.ToHexStringLower(hashBytes);
        }

        public string GetAlgorithmName()
        {
            return "CRC-32";
        }
    }
}


