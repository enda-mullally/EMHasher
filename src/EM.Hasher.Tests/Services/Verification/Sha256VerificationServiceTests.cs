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
    private const string TestFilesDir = "TestFiles";
    private const string VerificationDir = "Verification";
    private const string TargetFileName = "TestFile.bin";

    private const string CalculatedHash =
        "1be90ba8e2bb29edeec06ccfbbb295740857df787501744c0c4fbda157ecb21f";

    private static Sha_256VerificationService CreateSut()
    {
        return new Sha_256VerificationService();
    }

    private static string GetScenarioTargetPath(string scenario)
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(currentDirectory, TestFilesDir, VerificationDir, scenario, TargetFileName);
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
        var filePath = GetScenarioTargetPath(Path.Combine("does-not-exist", "nested"));

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
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(currentDirectory, TestFilesDir, TargetFileName);

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
        var filePath = GetScenarioTargetPath("Match");

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash);

        // Assert
        result.VerificationHashFound.Should().BeTrue();
        result.IsHashMatching.Should().BeTrue();
        result.HashVerificationDescription.Should().Contain("TestFile.sha256");
    }

    [TestMethod]
    public async Task VerifyAsync_MatchingHash_IsCaseInsensitiveAsync()
    {
        // Arrange
        var sut = CreateSut();
        var filePath = GetScenarioTargetPath("CaseInsensitive");

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
        var filePath = GetScenarioTargetPath("BinaryPrefix");

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
        var filePath = GetScenarioTargetPath("Mismatch");

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
        var filePath = GetScenarioTargetPath("DifferentFile");

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
        var filePath = GetScenarioTargetPath("MalformedLines");

        // Act
        var result = await sut.VerifyAsync(filePath, CalculatedHash);

        // Assert
        result.VerificationHashFound.Should().BeTrue();
        result.IsHashMatching.Should().BeTrue();
    }
}
