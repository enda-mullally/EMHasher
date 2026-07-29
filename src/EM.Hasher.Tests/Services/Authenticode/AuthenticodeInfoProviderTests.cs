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
    
    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow((string?)null)]
    public async Task GetAuthenticodeInfoAsync_NullOrWhiteSpaceFileName_ReturnsNotSignedAsync(string? fileName)
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.GetAuthenticodeInfoAsync(fileName!);

        // Assert
        result.Should().NotBeNull();
        result.IsSigned.Should().BeFalse();
    }
}
