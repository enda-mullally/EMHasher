using System;
using System.IO;
using System.Threading.Tasks;
using EM.Hasher.Services.File;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.File;

[TestClass]
public class FileDetailsProviderTests
{
    private const string TestFilesDir = "TestFiles";

    private static FileDetailsProvider CreateSut()
    {
        return new FileDetailsProvider();
    }

    private static string GetTestFilePath(string fileName)
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(currentDirectory, TestFilesDir, fileName);
    }

    [TestMethod]
    public async Task GetFileDetailsAsync_ExistingFile_ReturnsPopulatedDetailsAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = GetTestFilePath("TestFile.bin");

        // Act
        var result = await sut.GetFileDetailsAsync(filePath);

        // Assert
        result.Should().NotBeNull();
        result!.FullFileName.Should().Be(filePath);
        result.FileName.Should().Be("TestFile.bin");
        result.FileSize.Should().NotBeNullOrWhiteSpace();
        result.FileCreated.Should().NotBeNullOrWhiteSpace();
        result.FileModified.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public async Task GetFileDetailsAsync_ZeroByteFile_ReturnsDetailsWithZeroSizeAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = GetTestFilePath("ZeroByteFile.bin");

        // Act
        var result = await sut.GetFileDetailsAsync(filePath);

        // Assert
        result.Should().NotBeNull();
        result!.FileName.Should().Be("ZeroByteFile.bin");
        result.FileSize.Should().Contain("0");
    }

    [TestMethod]
    public async Task GetFileDetailsAsync_NonExistentFile_ReturnsNullAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = GetTestFilePath("ThisFileDoesNotExist.bin");

        // Act
        var result = await sut.GetFileDetailsAsync(filePath);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetFileDetailsAsync_SignedFile_ReturnsDetailsAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = GetTestFilePath("TestSignedExe.bin");

        // Act
        var result = await sut.GetFileDetailsAsync(filePath);

        // Assert
        result.Should().NotBeNull();
        result!.FileName.Should().Be("TestSignedExe.bin");
        result.FullFileName.Should().Be(filePath);
        result.FileSize.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public void GetFileVersionAndFileProductVersion_NonExistentFile_ReturnsEmpty()
    {
        // Arrange
        var filePath = GetTestFilePath("ThisFileDoesNotExist.bin");

        // Act
        var (fileVersion, productVersion) = FileDetailsProvider.GetFileVersionAndFileProductVersion(filePath);

        // Assert
        fileVersion.Should().BeEmpty();
        productVersion.Should().BeEmpty();
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow((string?)null)]
    public void GetFileVersionAndFileProductVersion_NullOrWhiteSpaceFileName_ReturnsEmpty(string? fileName)
    {
        // Act
        var (fileVersion, productVersion) = FileDetailsProvider.GetFileVersionAndFileProductVersion(fileName!);

        // Assert
        fileVersion.Should().BeEmpty();
        productVersion.Should().BeEmpty();
    }

    [TestMethod]
    public void GetFileVersionAndFileProductVersion_FileWithoutVersionInfo_ReturnsEmpty()
    {
        // Arrange
        var filePath = GetTestFilePath("TestFile.bin");

        // Act
        var (fileVersion, productVersion) = FileDetailsProvider.GetFileVersionAndFileProductVersion(filePath);

        // Assert
        fileVersion.Should().BeEmpty();
        productVersion.Should().BeEmpty();
    }
}
