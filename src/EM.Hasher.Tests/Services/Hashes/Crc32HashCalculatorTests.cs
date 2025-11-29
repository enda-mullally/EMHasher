using System;
using System.Threading.Tasks;
using EM.Hasher.Services.Hashes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Hashes
{
    [TestClass]
    public class Crc32HashCalculatorTests
    {
        private const string TestFilesDir = "TestFiles";

        [TestMethod]
        public async Task ZeroByteFileHash_WorksAsync()
        {
            // Arrange
            var sut = new Crc32HashCalculator();

            // Act
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "ZeroByteFile.bin");
            var hash = await sut.CalculateHashAsync(filePath);

            // Assert
            hash.Should().Be("00000000");
        }

        [TestMethod]
        public async Task TestFileHash_WorksAsync()
        {
            // Arrange
            var sut = new Crc32HashCalculator();

            // Act
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "TestFile.bin");
            var hash = await sut.CalculateHashAsync(filePath);

            // Assert
            hash.Should().Be("fd657a12");
        }
    }
}
