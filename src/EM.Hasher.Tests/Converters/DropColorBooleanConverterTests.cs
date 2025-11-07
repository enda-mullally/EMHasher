using EM.Hasher.Converters;
using FluentAssertions;
using Microsoft.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Converters
{
    [TestClass]
    public class DropColorBooleanConverterTests
    {
        [TestMethod]
        public void DropColorBooleanConverter_True_Works()
        {
            // Arrange
            var sut = new DropColorBooleanConverter();

            // Act
            var result = sut.Convert(true, typeof(bool), null!, string.Empty);
            
            // Assert
            result.Should().Be(Colors.Green);
        }

        [TestMethod]
        public void DropColorBooleanConverter_False_Works()
        {
            // Arrange
            var sut = new DropColorBooleanConverter();

            // Act
            var result = sut.Convert(false, typeof(bool), null!, string.Empty);

            // Assert
            result.Should().Be(Colors.Gray);
        }
    }
}
