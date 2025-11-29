using System;
using EM.Hasher.Services.Hashes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Hashes
{
    [TestClass]
    public class Sha512HashCalculatorTests
    {
        private const string TestFilesDir = "TestFiles";

        [TestMethod]
        public void ZeroByteFileHash_Works()
        {
            // Arrange
            var sut = new Sha512HashCalculator();

            // Act
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "ZeroByteFile.bin");
            var hash = sut.CalculateHashAsync(filePath).Result;

            // Assert
            hash.Should().Be("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e");
        }

        [TestMethod]
        public void TestFileHash_Works()
        {
            // Arrange
            var sut = new Sha512HashCalculator();

            // Act
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(currentDirectory, TestFilesDir, "TestFile.bin");
            var hash = sut.CalculateHashAsync(filePath).Result;

            // Assert
            hash.Should().Be("d27dbe6b57d3671f510d9bf763096d6ef0d2576ce5f4276c1c0aa99aafca1c178b62d80f73a62a20207e6f5ab85b71d004c4002390de5346f5def09d979b64b3");
        }
    }
}
