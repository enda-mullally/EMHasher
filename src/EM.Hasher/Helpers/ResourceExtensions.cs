/*
 * EM Hasher
 * Copyright © 2025-2026 Enda Mullally (em.apps@outlook.ie)
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

global using Res = EM.Hasher.Helpers.ResourceExtensions;

using System;
using System.Diagnostics;
using Microsoft.Windows.ApplicationModel.Resources;

namespace EM.Hasher.Helpers;

public static class ResourceExtensions
{
    private static readonly ResourceLoader _resourceLoader = new();

    public static string GetLocalized(this string resourceKey) => _resourceLoader.GetString(resourceKey);

    public static string WithPlaceholder(this string s, string token, string? value)
    {
        WarnIfMissing(s, token);

        var key = "{" + token + "}";
        return s.Contains(key, StringComparison.Ordinal)
            ? s.Replace(key, value ?? string.Empty)
            : s;
    }

    [Conditional("DEBUG")]
    private static void WarnIfMissing(string s, string key)
    {
        if (!s.Contains(key, StringComparison.Ordinal))
        {
            Debug.WriteLine($"[Res] Placeholder '{key}' not found in string.");
        }
    }
}
