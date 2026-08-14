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

public class HashProgressCalculator : IHashProgressCalculator
{
    private int _lastReportedPercentage = -1;

    public void Reset()
    {
        _lastReportedPercentage = -1;
    }

    public void Report(long processedBytes, long totalBytes, IProgress<int>? progress)
    {
        if (progress is null || totalBytes <= 0)
        {
            return;
        }

        if (processedBytes < 0)
        {
            processedBytes = 0;
        }
        else if (processedBytes > totalBytes)
        {
            processedBytes = totalBytes;
        }

        var percentage = (int)(processedBytes * 100 / totalBytes);

        if (percentage <= _lastReportedPercentage)
        {
            return;
        }

        _lastReportedPercentage = percentage;

        progress.Report(percentage);
    }
}
