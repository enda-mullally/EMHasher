/*
 * EM Hasher
 * Copyright © 2026 Enda Mullally (em.apps@outlook.ie)
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
using System.Security.Cryptography;
using System.Threading.Tasks;
using EM.Hasher.Services.Hashes.Progress;

namespace EM.Hasher.Services.Hashes;

public class Sha1_HashCalculator(IHashProgressCalculator progressCalculator) : IHashCalculator
{
    public async Task<string> CalculateHashAsync(string fileName, IProgress<int>? progress = null)
    {
        using var sha1 = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);

        await using var fileStream = new FileStream(fileName,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            IHashCalculator.BufferSize,
            useAsync: true);

        using var bufferedStream = new BufferedStream(fileStream, IHashCalculator.BufferSize);

        var totalBytes = fileStream.Length;
        var processedBytes = 0L;

        progressCalculator.Reset();
        progressCalculator.Report(processedBytes, totalBytes, progress);

        var buffer = new byte[IHashCalculator.BufferSize];
        int bytesRead;

        while ((bytesRead = await bufferedStream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
        {
            sha1.AppendData(buffer.AsSpan(0, bytesRead));

            processedBytes += bytesRead;

            progressCalculator.Report(processedBytes, totalBytes, progress);
        }

        var hashBytes = sha1.GetHashAndReset();

        return Convert.ToHexStringLower(hashBytes);
    }

    public string GetAlgorithmName()
    {
        return "SHA-1";
    }
}
