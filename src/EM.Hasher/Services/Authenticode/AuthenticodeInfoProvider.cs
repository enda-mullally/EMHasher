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
using System.Linq;
using System.Threading.Tasks;
using AuthenticodeExaminer;
using EM.Hasher.Models;
using EM.Hasher.Services.Parsers;
using Humanizer;

namespace EM.Hasher.Services.Authenticode;

public partial class AuthenticodeInfoProvider(IKeyValueDnParser dnParser) : IAuthenticodeInfoProvider
{
    private readonly IKeyValueDnParser _dnParser = dnParser;
    
    public async Task<AuthenticodeInfoModel> GetAuthenticodeInfoAsync(string fileName)
    {
        return await Task.Run(() =>
        {
            return GetAuthenticodeInfo(fileName);
        });
    }

    private AuthenticodeInfoModel GetAuthenticodeInfo(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(fileName))
        {
            return new AuthenticodeInfoModel
            {
                IsSigned = false
            };
        }

        try
        {
            var inspector = new FileInspector(fileName);
            var validationResult = inspector.Validate();
            var signatures = inspector.GetSignatures();
            var signingCert = signatures.FirstOrDefault();
            
            var signerSubject = signingCert != null ? signingCert.SigningCertificate?.Subject : string.Empty;
            var signer = signerSubject != string.Empty
                ? _dnParser.Load(signerSubject!)
                           .GetFirstFoundValue("CN", "O")
                           .Trim('"')
                : string.Empty;
                        
            var issuer = signingCert != null ? signingCert.SigningCertificate?.Issuer : string.Empty;
            issuer = issuer != string.Empty
                ? _dnParser.Load(issuer!)
                           .GetFirstFoundValue("CN", "O")
                           .Trim('"')
                : string.Empty;

            var timestamp = signingCert != null ? signingCert.TimestampSignatures.FirstOrDefault() : null;
            var timestampDate = timestamp != null ? timestamp.TimestampDateTime : DateTimeOffset.MinValue;
            var hasTimestamp = timestampDate != DateTimeOffset.MinValue;

            return new AuthenticodeInfoModel
            {
                IsSigned = validationResult == SignatureCheckResult.Valid,
                Issuer = issuer,
                Signer = signer,
                IsTimeStamped = hasTimestamp,
                SigningTime = hasTimestamp
                    ? $"{timestampDate:f} ({timestampDate.Humanize()})"
                    : string.Empty
            };
        }
        catch
        {
            return new AuthenticodeInfoModel
            {
                IsSigned = false
            };
        }
    }
}