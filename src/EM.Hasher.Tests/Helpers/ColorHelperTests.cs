using EM.Hasher.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Helpers;

[TestClass]
public class ColorHelperTests
{
    [TestMethod]
    public void GetColorFromHex_OpaqueRed_ReturnsExpectedArgb()
    {
        // Act
        var color = ColorHelper.GetColorFromHex("#FFFF0000");

        // Assert
        color.A.Should().Be(255);
        color.R.Should().Be(255);
        color.G.Should().Be(0);
        color.B.Should().Be(0);
    }

    [TestMethod]
    public void GetColorFromHex_RgbWithoutAlpha_DefaultsToOpaque()
    {
        // Act
        var color = ColorHelper.GetColorFromHex("#00FF00");

        // Assert
        color.A.Should().Be(255);
        color.R.Should().Be(0);
        color.G.Should().Be(255);
        color.B.Should().Be(0);
    }

    [TestMethod]
    public void GetColorFromHex_SemiTransparentBlue_ReturnsExpectedArgb()
    {
        // Act
        var color = ColorHelper.GetColorFromHex("#800000FF");

        // Assert
        color.A.Should().Be(128);
        color.R.Should().Be(0);
        color.G.Should().Be(0);
        color.B.Should().Be(255);
    }

    [TestMethod]
    public void GetColorFromHex_InvalidHex_Throws()
    {
        // Act
        var act = () => ColorHelper.GetColorFromHex("not-a-color");

        // Assert
        act.Should().Throw<System.Exception>();
    }
}
