using EM.Hasher.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Models;

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
        var fileVersion = "1.0.0";
        var fileProductVersion = "1.0.0+140d4e78-6d3b-4539-bc81-d12790c6efef";

        var expectedToString =
            $"FileName={fileName}, FullFileName={fullFileName}, FileSize={fileSize}, FileCreated={fileCreated}, FileModified={fileModified}, FileVersion={fileVersion}, FileProductVersion={fileProductVersion}";

        // Act
        var sut = new FileDetailsModel
        {
            FileName = fileName,
            FullFileName = fullFileName,
            FileSize = fileSize,
            FileCreated = fileCreated,
            FileModified = fileModified,
            FileVersion = fileVersion,
            FileProductVersion = fileProductVersion
        };

        // Assert
        sut.FileName.Should().Be(fileName);
        sut.FullFileName.Should().Be(fullFileName);
        sut.FileSize.Should().Be(fileSize);
        sut.FileCreated.Should().Be(fileCreated);
        sut.FileModified.Should().Be(fileModified);
        sut.FileVersion.Should().Be(fileVersion);
        sut.FileProductVersion.Should().Be(fileProductVersion);
        sut.ToString().Should().Be(expectedToString);
    }
}
