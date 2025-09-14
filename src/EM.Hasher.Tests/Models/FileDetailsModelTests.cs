using EM.Hasher.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Models
{
    [TestClass]
    public class FileDetailsModelTests
    {
        [TestMethod]
        public void FileDetailsModel_Works()
        {
            // Arrange
            var fileName = "test.txt";
            var fullFileName = @"C:\temp\test.txt";
            var fileSize = "1 KB";
            var fileCreated = "2024-01-01 10:00:00";
            var fileModified = "2024-01-02 11:00:00";
            var expectedToString =
                $"FileName={fileName}, FullFileName={fullFileName}, FileSize={fileSize}, FileCreated={fileCreated}, FileModified={fileModified}";

            // Act
            var sut = new FileDetailsModel
            {
                FileName = fileName,
                FullFileName = fullFileName,
                FileSize = fileSize,
                FileCreated = fileCreated,
                FileModified = fileModified
            };

            // Assert
            sut.FileName.Should().Be(fileName);
            sut.FullFileName.Should().Be(fullFileName);
            sut.FileSize.Should().Be(fileSize);
            sut.FileCreated.Should().Be(fileCreated);
            sut.FileModified.Should().Be(fileModified);
            sut.ToString().Should().Be(expectedToString);
        }
    }
}
