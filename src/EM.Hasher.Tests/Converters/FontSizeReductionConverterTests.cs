using EM.Hasher.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Converters;

[TestClass]
public class FontSizeResizeConverterTests
{
    [TestMethod]
    public void FontSizeResizeConverter_Reduce_Works()
    {
        // Arrange
        var sut = new FontSizeResizeConverter();

        // Act
        var result = sut.Convert(18.0d, typeof(double), "-2", string.Empty);

        // Assert
        result.Should().Be(16.0d);
    }

    [TestMethod]
    public void FontSizeResizeConverter_Increase_Works()
    {
        // Arrange
        var sut = new FontSizeResizeConverter();

        // Act
        var result = sut.Convert(18.0d, typeof(double), "3", string.Empty);

        // Assert
        result.Should().Be(21.0d);
    }
}
