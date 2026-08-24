using System;
using System.Threading.Tasks;
using EM.Hasher.Services.Hashes;
using EM.Hasher.Services.Hashes.Progress;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Hashes;

[TestClass]
public class Sha3_256HashCalculatorTests
{
    private const string TestFilesDir = "TestFiles";

    [TestMethod]
    public void GetAlgorithmName_Works()
    {
        // Arrange
        var sut = new Sha3_256HashCalculator(new HashProgressCalculator());

        // Assert
        sut.GetAlgorithmName().Should().Be("SHA3-256");
    }

    [TestMethod]
    public async Task ZeroByteFileHash_WorksAsync()
    {
        // Arrange
        var sut = new Sha3_256HashCalculator(new HashProgressCalculator());

        // Act
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "ZeroByteFile.bin");
        var hash = await sut.CalculateHashAsync(filePath);

        // Assert
        hash.Should().Be("a7ffc6f8bf1ed76651c14756a061d662f580ff4de43b49fa82d80a4b80f8434a");
    }

    [TestMethod]
    public async Task TestFileHash_WorksAsync()
    {
        // Arrange
        var sut = new Sha3_256HashCalculator(new HashProgressCalculator());

        // Act
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "TestFile.bin");
        var hash = await sut.CalculateHashAsync(filePath);

        // Assert
        hash.Should().Be("6fa784f778ec53fb14045fe6684727e34b606b6c809b327bc9a02081651c805f");
    }
}
