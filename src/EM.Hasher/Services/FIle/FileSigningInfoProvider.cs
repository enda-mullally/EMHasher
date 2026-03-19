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
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using EM.Hasher.Models;
using EM.Hasher.Services.Parsers;

namespace EM.Hasher.Services.File;

public partial class FileSigningInfoProvider(IKeyValueDnParser dnParser) : IFileSigningInfoProvider
{
    private const int ImageDirEntrySecurity = 4;
    private const int WinCertTypePkcsSignedData = 2;
    private const int DataDirectorySize = 128; // 16 entries * 8 bytes
    private const int DataDirectoryEntrySize = 8;
    private readonly IKeyValueDnParser _dnParser = dnParser;

    public async Task<FileSigningInfo> GetSigningInfoAsync(string fileName)
    {
        return await Task.Run(() =>
        {
            return GetSigningInfoCore(fileName);
        });
    }

    private FileSigningInfo GetSigningInfoCore(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(fileName))
        {
            return new FileSigningInfo
            {
                IsSigned = false
            };
        }

        try
        {
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (!TryGetCertificateTableOffset(fs, out var certTableFileOffset, out var certTableSize))
            {
                return new FileSigningInfo
                {
                    IsSigned = false
                };
            }

            if (certTableSize < 8)
            {
                return new FileSigningInfo
                {
                    IsSigned = false
                };
            }

            fs.Seek(certTableFileOffset, SeekOrigin.Begin);
            using var reader = new BinaryReader(fs);

            var dwLength = reader.ReadInt32();
            if (dwLength < 8 || dwLength > certTableSize)
            {
                return new FileSigningInfo
                {
                    IsSigned = false
                };
            }

            _ = reader.ReadInt16(); // wRevision
            var wCertificateType = reader.ReadInt16();
            if (wCertificateType != WinCertTypePkcsSignedData)
            {
                return new FileSigningInfo
                {
                    IsSigned = false
                };
            }

            var pkcs7Length = dwLength - 8;
            var pkcs7Blob = reader.ReadBytes(pkcs7Length);
            if (pkcs7Blob.Length != pkcs7Length)
            {
                return new FileSigningInfo
                {
                    IsSigned = false
                };
            }

            return GetSignerAndIssuerFromPkcs7(pkcs7Blob);
        }
        catch
        {
            return new FileSigningInfo
            {
                IsSigned = false
            };
        }
    }

    private FileSigningInfo GetSignerAndIssuerFromPkcs7(byte[] pkcs7Blob)
    {
        try
        {
            var signedCms = new SignedCms();
            signedCms.Decode(pkcs7Blob);

            if (signedCms.SignerInfos.Count == 0)
            {
                return new FileSigningInfo
                {
                    IsSigned = false,
                    IsTrusted = false
                };
            }

            var signerInfo = signedCms.SignerInfos[0];
            var signerCert = signerInfo.Certificate;
            if (signerCert == null)
            {
                return new FileSigningInfo
                {
                    IsSigned = true,
                    IsTrusted = false
                };
            }

            // Build an X509 chain to verify whether the certificate chains to a trusted root
            using var chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreNotTimeValid;

            var chainOk = false;
            try
            {
                chainOk = chain.Build(signerCert);
            }
            catch
            {
                chainOk = false;
            }

            var signer = _dnParser
                .Load(signerCert.Subject)
                .GetFirstFoundValue("CN", "O")
                .Trim('"');

            var issuer = _dnParser
                .Load(signerCert.Issuer)
                .GetFirstFoundValue("CN", "O")
                .Trim('"');

            return new FileSigningInfo
            {
                Signer = signer ?? string.Empty,
                Issuer = issuer ?? string.Empty,
                IsTrusted = chainOk,
                IsSigned = chainOk && !(string.IsNullOrWhiteSpace(signer) &&
                                        string.IsNullOrWhiteSpace(issuer))
            };
        }
        catch
        {
            return new FileSigningInfo
            {
                IsSigned = false,
                IsTrusted = false
            };
        }
    }
}