using System;
using EM.Hasher.Services.Hashes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Hashes
{
    [TestClass]
    public class Sha256HashCalculatorTests
    {
        private const string TestFilesDir = "TestFiles";

        [TestMethod]
        public void ZeroByteFileHash_Works()
        {
            // Arrange
            var sut = new Sha256HashCalculator();

            // Act
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "ZeroByteFile.bin");
            var hash = sut.CalculateHashAsync(filePath).Result;

            // Assert
            hash.Should().Be("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
        }

        [TestMethod]
        public void TestFileHash_Works()
        {
            // Arrange
            var sut = new Sha256HashCalculator();

            // Act
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "TestFile.bin");
            var hash = sut.CalculateHashAsync(filePath).Result;

            // Assert
            hash.Should().Be("1be90ba8e2bb29edeec06ccfbbb295740857df787501744c0c4fbda157ecb21f");
        }
    }
}
