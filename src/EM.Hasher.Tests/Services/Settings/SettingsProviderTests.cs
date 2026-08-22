using EM.Hasher.Services.Settings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Settings;

[TestClass]
public class SettingsProviderTests
{
    [TestMethod]
    [TestCategory("UI")]
    public void Settings_Bools_True_Works()
    {
        var settingsProvider = new SettingsProvider
        {
            IsMd5_Enabled = true,
            IsSha256_Enabled = true,
            IsSha3_256_Enabled = true,
            IsUppercaseHashValues = true
        };

        settingsProvider.IsMd5_Enabled.Should().BeTrue();
        settingsProvider.IsSha256_Enabled.Should().BeTrue();
        settingsProvider.IsSha3_256_Enabled.Should().BeTrue();
        settingsProvider.IsUppercaseHashValues.Should().BeTrue();
    }

    [TestMethod]
    [TestCategory("UI")]
    public void Settings_Bools_False_Works()
    {
        var settingsProvider = new SettingsProvider
        {
            IsMd5_Enabled = false,
            IsSha256_Enabled = false,
            IsSha3_256_Enabled = false,
            IsUppercaseHashValues = false
        };

        settingsProvider.IsMd5_Enabled.Should().BeFalse();
        settingsProvider.IsSha256_Enabled.Should().BeFalse();
        settingsProvider.IsSha3_256_Enabled.Should().BeFalse();
        settingsProvider.IsUppercaseHashValues.Should().BeFalse();
    }

    [TestMethod]
    [TestCategory("UI")]
    public void Settings_Theme_Works()
    {
        var settingsProvider = new SettingsProvider
        {
            SelectedTheme = (int)Microsoft.UI.Xaml.ElementTheme.Dark
        };

        settingsProvider.SelectedTheme.Should().Be((int)Microsoft.UI.Xaml.ElementTheme.Dark);

        settingsProvider = new SettingsProvider
        {
            SelectedTheme = (int)Microsoft.UI.Xaml.ElementTheme.Light
        };

        settingsProvider.SelectedTheme.Should().Be((int)Microsoft.UI.Xaml.ElementTheme.Light);

        settingsProvider = new SettingsProvider
        {
            SelectedTheme = (int)Microsoft.UI.Xaml.ElementTheme.Default
        };

        settingsProvider.SelectedTheme.Should().Be((int)Microsoft.UI.Xaml.ElementTheme.Default);
    }
}
