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
using System.Threading.Tasks;
using EM.Hasher.Models;

namespace EM.Hasher.Services.Verification;

public class Sha256VerificationService : IHashVerificationService
{
    private const string HashFileSearchPattern = "*.sha256";

    public async Task<HashVerificationModel> VerifyAsync(string fileName, string calculatedHash)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(calculatedHash))
        {
            return new HashVerificationModel();
        }

        var directory = Path.GetDirectoryName(fileName);

        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            return new HashVerificationModel();
        }

        var hashFiles = Directory.GetFiles(directory, HashFileSearchPattern);

        if (hashFiles.Length == 0)
        {
            return new HashVerificationModel();
        }

        var targetFileName = Path.GetFileName(fileName);

        foreach (var hashFile in hashFiles)
        {
            var hashFileName = Path.GetFileName(hashFile);
            var lines = await System.IO.File.ReadAllLinesAsync(hashFile);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Expected format: "<hash>  <fileName>" (whitespace separated).
                var parts = line.Trim().Split(
                    new[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                {
                    continue;
                }

                var expectedHash = parts[0];

                // The filename in the hash file may be prefixed with '*' (binary mode)
                // and can contain spaces, so re-join the remaining parts.
                var expectedFileName = string.Join(' ', parts, 1, parts.Length - 1)
                    .TrimStart('*');
                expectedFileName = Path.GetFileName(expectedFileName);

                // Only treat this line as verification info when the filename matches.
                if (!expectedFileName.Equals(targetFileName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (expectedHash.Equals(calculatedHash, StringComparison.OrdinalIgnoreCase))
                {
                    return new HashVerificationModel
                    {
                        VerificationHashFound = true,
                        IsHashMatching = true,
                        HashVerificationDescription =
                            $"Verification passed. Matching hash found in '{hashFileName}'"
                    };
                }
                else
                {
                    return new HashVerificationModel
                    {
                        VerificationHashFound = true,
                        IsHashMatching = false,
                        HashVerificationDescription =
                            $"Verification failed! The hash found in '{hashFileName}' does not match."
                    };
                }
            }
        }

        return new HashVerificationModel
        {
            VerificationHashFound = false
        };
    }
}
