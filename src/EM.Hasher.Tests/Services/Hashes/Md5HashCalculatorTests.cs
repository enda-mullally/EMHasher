using System;
using EM.Hasher.Services.Hashes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Hashes
{
    [TestClass]
    public class Md5HashCalculatorTests
    {
        private const string TestFilesDir = "TestFiles";

        [TestMethod]
        public void ZeroByteFileHash_Works()
        {
            // Arrange
            var sut = new Md5HashCalculator();

            // Act
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "ZeroByteFile.bin");
            var hash = sut.CalculateHashAsync(filePath).Result;

            // Assert
            hash.Should().Be("d41d8cd98f00b204e9800998ecf8427e");
        }

        [TestMethod]
        public void TestFileHash_Works()
        {
            // Arrange
            var sut = new Md5HashCalculator();

            // Act
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "TestFile.bin");
            var hash = sut.CalculateHashAsync(filePath).Result;

            // Assert
            hash.Should().Be("0daddf8ce1b2e740a1331ade98a6637e");
        }
    }
}
