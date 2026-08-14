using System;
using System.Threading.Tasks;
using EM.Hasher.Services.Hashes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Hashes;

[TestClass]
public class Blake3HashCalculatorTests
{
    private const string TestFilesDir = "TestFiles";

    [TestMethod]
    public async Task ZeroByteFileHash_WorksAsync()
    {
        // Arrange
        var sut = new Blake3HashCalculator(new HashProgressCalculator());

        // Act
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "ZeroByteFile.bin");
        var hash = await sut.CalculateHashAsync(filePath);

        // Assert
        hash.Should().Be("af1349b9f5f9a1a6a0404dea36dcc9499bcb25c9adc112b7cc9a93cae41f3262");
    }

    [TestMethod]
    public async Task TestFileHash_WorksAsync()
    {
        // Arrange
        var sut = new Blake3HashCalculator(new HashProgressCalculator());

        // Act
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "TestFile.bin");
        var hash = await sut.CalculateHashAsync(filePath);

        // Assert
        hash.Should().Be("723ad76374913c8580267a7937b4d7434e00453ae1b2665984667e144920a386");
    }
}
