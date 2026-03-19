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

using System.IO;

namespace EM.Hasher.Services.File;

public partial class FileSigningInfoProvider
{
    private static bool TryGetCertificateTableOffset(FileStream fs, out long certTableFileOffset, out int certTableSize)
    {
        certTableFileOffset = 0;
        certTableSize = 0;

        if (fs.Length < 64)
        {
            return false;
        }

        fs.Seek(0x3C, SeekOrigin.Begin);
        var eLfanew = ReadInt32Le(fs);
        if (eLfanew < 0 || eLfanew > fs.Length - 24)
        {
            return false;
        }

        fs.Seek(eLfanew, SeekOrigin.Begin);
        if (ReadUInt32Le(fs) != 0x00004550) // "PE\0\0"
        {
            return false;
        }

        // COFF header: SizeOfOptionalHeader at offset 16
        fs.Seek(eLfanew + 4 + 16, SeekOrigin.Begin);
        var sizeOfOptionalHeader = ReadUInt16Le(fs);
        if (sizeOfOptionalHeader < DataDirectorySize)
        {
            return false;
        }

        // Data Directory entry 4 (Security) is at end of optional header - 128 + 4*8
        long dataDirStart = eLfanew + 4 + 20 + sizeOfOptionalHeader - DataDirectorySize;
        var securityEntryOffset = dataDirStart + (ImageDirEntrySecurity * DataDirectoryEntrySize);
        if (securityEntryOffset + 8 > fs.Length)
        {
            return false;
        }

        fs.Seek(securityEntryOffset, SeekOrigin.Begin);
        var virtualAddress = ReadUInt32Le(fs);
        var size = (int)ReadUInt32Le(fs);

        if (virtualAddress == 0 || size == 0 || virtualAddress > int.MaxValue)
        {
            return false;
        }

        certTableFileOffset = virtualAddress;
        certTableSize = size;
        return true;
    }

    private static int ReadInt32Le(Stream s)
    {
        var b1 = s.ReadByte();
        var b2 = s.ReadByte();
        var b3 = s.ReadByte();
        var b4 = s.ReadByte();

        if (b4 < 0)
        {
            return 0;
        }

        return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
    }

    private static uint ReadUInt32Le(Stream s)
    {
        return (uint)ReadInt32Le(s);
    }

    private static ushort ReadUInt16Le(Stream s)
    {
        var b1 = s.ReadByte();
        var b2 = s.ReadByte();

        if (b2 < 0)
        {
            return 0;
        }

        return (ushort)(b1 | (b2 << 8));
    }
}