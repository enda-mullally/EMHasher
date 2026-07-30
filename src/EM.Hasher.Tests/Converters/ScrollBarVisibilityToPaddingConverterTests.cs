using System;
using EM.Hasher.Converters;
using FluentAssertions;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Converters;

[TestClass]
public class ScrollBarVisibilityToPaddingConverterTests
{
    [TestMethod]
    public void Convert_Visible_ReturnsRightPadding()
    {
        // Arrange
        var sut = new ScrollBarVisibilityToPaddingConverter();

        // Act
        var result = sut.Convert(Visibility.Visible, typeof(Thickness), null!, string.Empty);

        // Assert
        result.Should().Be(new Thickness(0, 0, 20, 0));
    }

    [TestMethod]
    public void Convert_Collapsed_ReturnsZeroPadding()
    {
        // Arrange
        var sut = new ScrollBarVisibilityToPaddingConverter();

        // Act
        var result = sut.Convert(Visibility.Collapsed, typeof(Thickness), null!, string.Empty);

        // Assert
        result.Should().Be(new Thickness(0));
    }

    [TestMethod]
    public void Convert_NonVisibilityValue_ReturnsZeroPadding()
    {
        // Arrange
        var sut = new ScrollBarVisibilityToPaddingConverter();

        // Act
        var result = sut.Convert("not a visibility", typeof(Thickness), null!, string.Empty);

        // Assert
        result.Should().Be(new Thickness(0));
    }

    [TestMethod]
    public void ConvertBack_Throws_NotImplementedException()
    {
        // Arrange
        var sut = new ScrollBarVisibilityToPaddingConverter();

        // Act
        var act = () => sut.ConvertBack(new Thickness(0), typeof(Visibility), null!, string.Empty);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }
}
