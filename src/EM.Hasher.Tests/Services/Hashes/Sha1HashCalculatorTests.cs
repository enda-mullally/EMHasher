using System;
using System.Threading.Tasks;
using EM.Hasher.Services.Hashes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Hashes;

[TestClass]
public class Sha1HashCalculatorTests
{
    private const string TestFilesDir = "TestFiles";

    [TestMethod]
    public async Task ZeroByteFileHash_WorksAsync()
    {
        // Arrange
        var sut = new Sha1HashCalculator();

        // Act
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "ZeroByteFile.bin");
        var hash = await sut.CalculateHashAsync(filePath);

        // Assert
        hash.Should().Be("da39a3ee5e6b4b0d3255bfef95601890afd80709");
    }

    [TestMethod]
    public async Task TestFileHash_WorksAsync()
    {
        // Arrange
        var sut = new Sha1HashCalculator();

        // Act
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "TestFile.bin");
        var hash = await sut.CalculateHashAsync(filePath);

        // Assert
        hash.Should().Be("1308cc485f80b7d8b2edb068e1f8fc1f3bfc2d6e");
    }
}
