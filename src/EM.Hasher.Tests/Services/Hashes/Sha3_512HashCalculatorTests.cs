using System;
using System.Threading.Tasks;
using EM.Hasher.Services.Hashes;
using EM.Hasher.Services.Hashes.Progress;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Hashes;

[TestClass]
public class Sha3_512HashCalculatorTests
{
    private const string TestFilesDir = "TestFiles";

    [TestMethod]
    public void GetAlgorithmName_Works()
    {
        // Arrange
        var sut = new Sha3_512HashCalculator(new HashProgressCalculator());

        // Assert
        sut.GetAlgorithmName().Should().Be("SHA3-512");
    }

    [TestMethod]
    public async Task ZeroByteFileHash_WorksAsync()
    {
        // Arrange
        var sut = new Sha3_512HashCalculator(new HashProgressCalculator());

        // Act
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "ZeroByteFile.bin");
        var hash = await sut.CalculateHashAsync(filePath);

        // Assert
        hash.Should().Be("a69f73cca23a9ac5c8b567dc185a756e97c982164fe25859e0d1dcc1475c80a615b2123af1f5f94c11e3e9402c3ac558f500199d95b6d3e301758586281dcd26");
    }

    [TestMethod]
    public async Task TestFileHash_WorksAsync()
    {
        // Arrange
        var sut = new Sha3_512HashCalculator(new HashProgressCalculator());

        // Act
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "TestFile.bin");
        var hash = await sut.CalculateHashAsync(filePath);

        // Assert
        hash.Should().Be("a91a1f971305ac13f7f6e3834e6537b553f2938e00b5ea294e0b079dc8a66286dcac97179fc83f62f143574cbc43d66905ae3b3722b4ab0fa67d90885d52de56");
    }
}
