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
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using EM.Hasher.Models;

namespace EM.Hasher.Services.File
{
    public class FileSigningInfoProvider : IFileSigningInfoProvider
    {
        private const int ImageDirEntrySecurity = 4;
        private const int WinCertTypePkcsSignedData = 2;
        private const int DataDirectorySize = 128; // 16 entries * 8 bytes
        private const int DataDirectoryEntrySize = 8;

        public async Task<FileSigningInfo> GetSigningInfoAsync(string filePath)
        {
            return await Task.Run(() => GetSigningInfoCore(filePath)).ConfigureAwait(false);
        }

        private static FileSigningInfo GetSigningInfoCore(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !System.IO.File.Exists(filePath))
            {
                return new FileSigningInfo { IsSigned = false };
            }

            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (!TryGetCertificateTableOffset(fs, out long certTableFileOffset, out int certTableSize))
                {
                    return new FileSigningInfo { IsSigned = false };
                }

                if (certTableSize < 8)
                {
                    return new FileSigningInfo { IsSigned = false };
                }

                fs.Seek(certTableFileOffset, SeekOrigin.Begin);
                using var reader = new BinaryReader(fs);

                int dwLength = reader.ReadInt32();
                if (dwLength < 8 || dwLength > certTableSize)
                {
                    return new FileSigningInfo { IsSigned = false };
                }

                _ = reader.ReadInt16(); // wRevision
                short wCertificateType = reader.ReadInt16();
                if (wCertificateType != WinCertTypePkcsSignedData)
                {
                    return new FileSigningInfo { IsSigned = false };
                }

                int pkcs7Length = dwLength - 8;
                byte[] pkcs7Blob = reader.ReadBytes(pkcs7Length);
                if (pkcs7Blob.Length != pkcs7Length)
                {
                    return new FileSigningInfo { IsSigned = false };
                }

                return GetSignerAndIssuerFromPkcs7(pkcs7Blob);
            }
            catch
            {
                return new FileSigningInfo { IsSigned = false };
            }
        }

        private static bool TryGetCertificateTableOffset(FileStream fs, out long certTableFileOffset, out int certTableSize)
        {
            certTableFileOffset = 0;
            certTableSize = 0;

            if (fs.Length < 64)
            {
                return false;
            }

            fs.Seek(0x3C, SeekOrigin.Begin);
            int eLfanew = ReadInt32Le(fs);
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
            ushort sizeOfOptionalHeader = ReadUInt16Le(fs);
            if (sizeOfOptionalHeader < DataDirectorySize)
            {
                return false;
            }

            // Data Directory entry 4 (Security) is at end of optional header - 128 + 4*8
            long dataDirStart = eLfanew + 4 + 20 + sizeOfOptionalHeader - DataDirectorySize;
            long securityEntryOffset = dataDirStart + (ImageDirEntrySecurity * DataDirectoryEntrySize);
            if (securityEntryOffset + 8 > fs.Length)
            {
                return false;
            }

            fs.Seek(securityEntryOffset, SeekOrigin.Begin);
            uint virtualAddress = ReadUInt32Le(fs);
            int size = (int)ReadUInt32Le(fs);

            if (virtualAddress == 0 || size == 0 || virtualAddress > int.MaxValue)
            {
                return false;
            }

            certTableFileOffset = virtualAddress;
            certTableSize = size;
            return true;
        }

        private static FileSigningInfo GetSignerAndIssuerFromPkcs7(byte[] pkcs7Blob)
        {
            try
            {
                var signedCms = new SignedCms();
                signedCms.Decode(pkcs7Blob);

                if (signedCms.SignerInfos.Count == 0)
                {
                    return new FileSigningInfo { IsSigned = false };
                }

                SignerInfo signerInfo = signedCms.SignerInfos[0];
                X509Certificate2? signerCert = signerInfo.Certificate;
                if (signerCert == null)
                {
                    return new FileSigningInfo { IsSigned = true, Signer = string.Empty, Issuer = string.Empty };
                }

                string signer = signerCert.Subject;
                string issuer = signerCert.Issuer;

                return new FileSigningInfo
                {
                    IsSigned = true,
                    Signer = signer ?? string.Empty,
                    Issuer = issuer ?? string.Empty
                };
            }
            catch
            {
                return new FileSigningInfo { IsSigned = false };
            }
        }

        private static int ReadInt32Le(Stream s)
        {
            int b1 = s.ReadByte();
            int b2 = s.ReadByte();
            int b3 = s.ReadByte();
            int b4 = s.ReadByte();
            if (b4 < 0) return 0;
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
        }

        private static uint ReadUInt32Le(Stream s)
        {
            return (uint)ReadInt32Le(s);
        }

        private static ushort ReadUInt16Le(Stream s)
        {
            int b1 = s.ReadByte();
            int b2 = s.ReadByte();
            if (b2 < 0) return 0;
            return (ushort)(b1 | (b2 << 8));
        }
    }
}