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

namespace EM.Hasher.Services.Hashes;

/// <summary>
/// Calculates the completion percentage of a hash calculation and reports it,
/// throttled so a value is only pushed to <see cref="IProgress{T}"/> when the
/// whole-number percentage actually changes.
/// </summary>
public interface IHashProgressCalculator
{
    /// <summary>
    /// Resets internal state so the calculator can be reused for a new file.
    /// </summary>
    void Reset();

    /// <summary>
    /// Computes the percentage complete from the number of processed bytes
    /// relative to the total, reporting it via <paramref name="progress"/> only
    /// when the integer percentage has changed since the last report.
    /// </summary>
    /// <param name="processedBytes">Total bytes processed so far.</param>
    /// <param name="totalBytes">Total bytes to process.</param>
    /// <param name="progress">Optional progress sink to report the percentage to.</param>
    void Report(long processedBytes, long totalBytes, IProgress<int>? progress);
}
