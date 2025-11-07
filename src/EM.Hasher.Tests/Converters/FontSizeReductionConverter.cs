using EM.Hasher.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Converters
{
    [TestClass]
    public class FontSizeReductionConverterTests
    {
        [TestMethod]
        public void FontSizeReductionConverter_Works()
        {
            // Arrange
            var sut = new FontSizeReductionConverter();

            // Act
            var result = sut.Convert(18.0d, typeof(double), "-2", string.Empty);

            // Assert
            result.Should().Be(16.0d);
        }
    }
}
