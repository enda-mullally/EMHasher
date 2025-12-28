using EM.Hasher.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Converters
{
    [TestClass]
    public class AppThemeSettingsConverterTests
    {
        [TestMethod]
        public void AppThemeSettingsConverter_Convert_Works()
        {
            // Arrange
            var sut = new AppThemeSettingConverter();

            // Act
            var appSettingTheme = (int)Enums.AppThemeSetting.Dark;
            var result = sut.Convert(appSettingTheme, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be(2); // Item 2 in the combobox is "Dark"

            // Act
            appSettingTheme = (int)Enums.AppThemeSetting.Light;
            result = sut.Convert(appSettingTheme, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be(1);  // Item 1 in the combobox is "Light"

            // Act
            appSettingTheme = (int)Enums.AppThemeSetting.Default;
            result = sut.Convert(appSettingTheme, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be(0);  // Item 0 in the combobox is "Default"
        }

        [TestMethod]
        public void AppThemeSettingsConverter_ConvertBack_Works()
        {
            // Arrange
            var sut = new AppThemeSettingConverter();

            // Act
            var uiSelectedIndex = 0; // Item 0 in the combobox is "Default"
            var result = sut.ConvertBack(uiSelectedIndex, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be((int)Enums.AppThemeSetting.Default);

            // Act
            uiSelectedIndex = 1;    // Item 1 in the combobox is "Light"
            result = sut.ConvertBack(uiSelectedIndex, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be((int)Enums.AppThemeSetting.Light);

            // Act
            uiSelectedIndex = 2;    // Item 2 in the combobox is "Dark"
            result = sut.ConvertBack(uiSelectedIndex, typeof(int), string.Empty, string.Empty);

            // Assert
            result.Should().Be((int)Enums.AppThemeSetting.Dark);
        }
    }
}
