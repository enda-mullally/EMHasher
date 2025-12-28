using EM.Hasher.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Converters
{
    [TestClass]
    public class CasingSettingsConverterTests
    {
        [TestMethod]
        public void CasingSettingsConverter_Convert_Works()
        {
            // Arrange
            var sut = new CasingSettingConverter();

            // Act
            var boolValue = true;
            var result = sut.Convert(boolValue, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be(0); // Item 0 in the combobox is "Uppercase"

            // Act
            boolValue = false;
            result = sut.Convert(boolValue, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be(1); // Item 1 in the combobox is "Lowercase"
        }

        [TestMethod]
        public void AppThemeSettingsConverter_ConvertBack_Works()
        {
            // Arrange
            var sut = new CasingSettingConverter();

            // Act
            var uiSelectedIndex = 0; // Item 0 in the combobox is "Uppercase"
            var result = sut.ConvertBack(uiSelectedIndex, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be(true);

            // Act
            uiSelectedIndex = 1; // Item 1 in the combobox is "Lowercase"
            result = sut.ConvertBack(uiSelectedIndex, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be(false);
        }
    }
}
