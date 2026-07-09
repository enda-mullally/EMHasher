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
using System.Collections.Generic;

namespace EM.Hasher.Services.Parsers;

public class KeyValueDnParser : IKeyValueDnParser
{
    private Dictionary<string, string>? _values;

    public IKeyValueDnParser Load(string input)
    {
        _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(input))
        {
            return this;
        }

        var parts = input.Split(',');
        var prevKey = string.Empty;

        foreach (var part in parts)
        {
            var kv = part.Split('=', 2); // only split on first =

            if (kv.Length == 2)
            {
                var key = prevKey = kv[0].Trim();
                var value = kv[1].Trim();

                _values[key] = value;
            }
            else if (kv.Length == 1)
            {
                // this is part of the previous key
                _values[prevKey] += ", " + kv[0].Trim();
            }
        }

        return this;
    }

    public string GetFirstFoundValue(params string[] keys)
    {
        foreach (var key in keys)
        {
            if (_values != null &&
                !string.IsNullOrEmpty(key) &&
                _values.TryGetValue(key, out var value))
            {
                return value;
            }
        }

        return string.Empty;
    }
}