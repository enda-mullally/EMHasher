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
using EM.Hasher.Services.Authenticode;
using EM.Hasher.Services.Parsers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Authenticode;

[TestClass]
public class AuthenticodeInfoProviderTests
{
    private const string TestFilesDir = "TestFiles";

    private static AuthenticodeInfoProvider CreateSut()
    {
        return new AuthenticodeInfoProvider(new KeyValueDnParser());
    }

    private static string GetTestFilePath(string fileName)
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(currentDirectory, TestFilesDir, fileName);
    }

    [TestMethod]
    public async Task GetAuthenticodeInfoAsync_SignedFile_ReturnsSignedInfoAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = GetTestFilePath("TestSignedExe.bin");

        // Act
        var result = await sut.GetAuthenticodeInfoAsync(filePath);

        // Assert
        result.Should().NotBeNull();
        result.IsSigned.Should().BeTrue();
        result.Signer.Should().NotBeNullOrWhiteSpace();
        result.Issuer.Should().NotBeNullOrWhiteSpace();
        result.IsTimeStamped.Should().BeTrue();
        result.SigningTime.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task GetAuthenticodeInfoAsync_UnsignedFile_ReturnsNotSignedAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = GetTestFilePath("TestFile.bin");

        // Act
        var result = await sut.GetAuthenticodeInfoAsync(filePath);

        // Assert
        result.Should().NotBeNull();
        result.IsSigned.Should().BeFalse();
        result.Signer.Should().BeEmpty();
        result.Issuer.Should().BeEmpty();
        result.IsTimeStamped.Should().BeFalse();
        result.SigningTime.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetAuthenticodeInfoAsync_ZeroByteFile_ReturnsNotSignedAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = GetTestFilePath("ZeroByteFile.bin");

        // Act
        var result = await sut.GetAuthenticodeInfoAsync(filePath);

        // Assert
        result.Should().NotBeNull();
        result.IsSigned.Should().BeFalse();
    }

    [TestMethod]
    public async Task GetAuthenticodeInfoAsync_NonExistentFile_ReturnsNotSignedAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = GetTestFilePath("ThisFileDoesNotExist.bin");

        // Act
        var result = await sut.GetAuthenticodeInfoAsync(filePath);

        // Assert
        result.Should().NotBeNull();
        result.IsSigned.Should().BeFalse();
        result.Signer.Should().BeEmpty();
        result.Issuer.Should().BeEmpty();
        result.IsTimeStamped.Should().BeFalse();
        result.SigningTime.Should().BeEmpty();
    }

    // TODO: This runs fine locally, but fails on pipeline run (ci).
    // Likely an issue with my headless test runner (RunAllTestsAsync).
    // Will re-visit and fix for tests with parameters.

    //[TestMethod]
    //[DataRow("")]
    //[DataRow("   ")]
    //[DataRow((string?)null)]
    //public async Task GetAuthenticodeInfoAsync_NullOrWhiteSpaceFileName_ReturnsNotSignedAsync(string? fileName)
    //{
    //    // Arrange
    //    var sut = CreateSut();

    //    // Act
    //    var result = await sut.GetAuthenticodeInfoAsync(fileName!);

    //    // Assert
    //    result.Should().NotBeNull();
    //    result.IsSigned.Should().BeFalse();
    //}
}
