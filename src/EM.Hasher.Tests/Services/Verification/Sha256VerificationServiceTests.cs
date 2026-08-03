using System;
using System.IO;
using System.Threading.Tasks;
using EM.Hasher.Services.Verification;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Verification;

[TestClass]
public class Sha256VerificationServiceTests
{
    private string _testDirectory = string.Empty;

    private const string CalculatedHash =
        "9F86D081884C7D659A2FEAA0C55AD015A3BF4F1B2B0B822CD15D6C15B0F00A08";

    [TestInitialize]
    public void TestInitialize()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_testDirectory);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }

    private static Sha256VerificationService CreateSut()
    {
        return new Sha256VerificationService();
    }

    private string CreateFile(string fileName, string? contents = null)
    {
        var filePath = Path.Combine(_testDirectory, fileName);
        System.IO.File.WriteAllText(filePath, contents ?? string.Empty);
        return filePath;
    }

    [DataTestMethod]
    [DataRow("", "abc")]
    [DataRow("test.bin", "")]
    [DataRow(null, null)]
    public async Task VerifyAsync_NullOrEmptyArguments_ReturnsEmptyModelAsync(string? fileName, string? calculatedHash)
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.VerifyAsync(fileName!, calculatedHash!);

        // Assert
        result.VerificationHashFound.Should().BeFalse();
        result.IsHashMatching.Should().BeFalse();
    }

    [TestMethod]
    public async Task VerifyAsync_DirectoryDoesNotExist_ReturnsEmptyModelAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = Path.Combine(_testDirectory, "does-not-exist", "test.bin");

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash);

        // Assert
        result.VerificationHashFound.Should().BeFalse();
    }

    [TestMethod]
    public async Task VerifyAsync_NoHashFiles_ReturnsHashNotFoundAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = CreateFile("test.bin");

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash);

        // Assert
        result.VerificationHashFound.Should().BeFalse();
    }

    [TestMethod]
    public async Task VerifyAsync_MatchingHash_ReturnsMatchAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = CreateFile("test.bin");
        CreateFile("test.sha256", $"{CalculatedHash}  test.bin");

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash);

        // Assert
        result.VerificationHashFound.Should().BeTrue();
        result.IsHashMatching.Should().BeTrue();
        result.HashVerificationDescription.Should().Contain("test.sha256");
    }

    [TestMethod]
    public async Task VerifyAsync_MatchingHash_IsCaseInsensitiveAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = CreateFile("test.bin");
        CreateFile("test.sha256", $"{CalculatedHash.ToLowerInvariant()}  test.bin");

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash.ToUpperInvariant());

        // Assert
        result.VerificationHashFound.Should().BeTrue();
        result.IsHashMatching.Should().BeTrue();
    }

    [TestMethod]
    public async Task VerifyAsync_BinaryModePrefix_ReturnsMatchAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = CreateFile("test.bin");
        CreateFile("test.sha256", $"{CalculatedHash} *test.bin");

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash);

        // Assert
        result.VerificationHashFound.Should().BeTrue();
        result.IsHashMatching.Should().BeTrue();
    }

    [TestMethod]
    public async Task VerifyAsync_NonMatchingHash_ReturnsMismatchAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = CreateFile("test.bin");
        CreateFile("test.sha256", $"0000000000000000000000000000000000000000000000000000000000000000  test.bin");

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash);

        // Assert
        result.VerificationHashFound.Should().BeTrue();
        result.IsHashMatching.Should().BeFalse();
        result.HashVerificationDescription.Should().Contain("does not match");
    }

    [TestMethod]
    public async Task VerifyAsync_HashFileForDifferentFile_ReturnsHashNotFoundAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = CreateFile("test.bin");
        CreateFile("test.sha256", $"{CalculatedHash}  other.bin");

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash);

        // Assert
        result.VerificationHashFound.Should().BeFalse();
    }

    [TestMethod]
    public async Task VerifyAsync_IgnoresBlankAndMalformedLinesAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = CreateFile("test.bin");
        var contents =
            Environment.NewLine +
            "malformed-line-without-filename" + Environment.NewLine +
            $"{CalculatedHash}  test.bin" + Environment.NewLine;
        CreateFile("test.sha256", contents);

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash);

        // Assert
        result.VerificationHashFound.Should().BeTrue();
        result.IsHashMatching.Should().BeTrue();
    }
}
