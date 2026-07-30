using EM.Hasher.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Helpers;

[TestClass]
public class FileNameShortenerTests
{
    [TestMethod]
    public void ShortenFilename_ShorterThanMaxLength_ReturnsUnchanged()
    {
        // Arrange
        var filename = "report.pdf";

        // Act
        var result = FileNameShortener.ShortenFilename(filename, 50);

        // Assert
        result.Should().Be(filename);
    }

    [TestMethod]
    public void ShortenFilename_EqualToMaxLength_ReturnsUnchanged()
    {
        // Arrange
        var filename = "report.pdf";

        // Act
        var result = FileNameShortener.ShortenFilename(filename, filename.Length);

        // Assert
        result.Should().Be(filename);
    }

    [TestMethod]
    public void ShortenFilename_LongerThanMaxLength_InsertsEllipsis()
    {
        // Arrange
        var filename = "ThisIsAVeryLongFileNameThatShouldBeShortened.txt";

        // Act
        var result = FileNameShortener.ShortenFilename(filename, 20);

        // Assert
        result.Should().Contain("...");
    }

    [TestMethod]
    public void ShortenFilename_LongerThanMaxLength_PreservesExtension()
    {
        // Arrange
        var filename = "ThisIsAVeryLongFileNameThatShouldBeShortened.txt";

        // Act
        var result = FileNameShortener.ShortenFilename(filename, 20);

        // Assert
        result.Should().EndWith(".txt");
    }

    [TestMethod]
    public void ShortenFilename_LongerThanMaxLength_ResultWithinMaxLength()
    {
        // Arrange
        var filename = "ThisIsAVeryLongFileNameThatShouldBeShortened.txt";

        // Act
        var result = FileNameShortener.ShortenFilename(filename, 20);

        // Assert
        result.Length.Should().BeLessThanOrEqualTo(20);
    }

    [TestMethod]
    public void ShortenFilename_LongerThanMaxLength_KeepsStartAndEnd()
    {
        // Arrange
        var filename = "AbcdefghijklmnopqrstuvwxyzFileName.log";

        // Act
        var result = FileNameShortener.ShortenFilename(filename, 20);

        // Assert
        result.Should().StartWith("Abc");
        result.Should().EndWith(".log");
    }
}
