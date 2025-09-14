using EM.Hasher.Messages.UI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Messages.UI
{
    [TestClass]
    public class SettingsChangedMessageTests
    {
        [TestMethod]
        public void SettingsChangedMessage_Works()
        {
            // Arrange
            var sut = new SettingsChangedMessage(
                new System.Collections.Generic.Dictionary<string, bool>()
                {
                    { "MD5", true },
                    { "SHA-256", true },
                    { "SHA-512", true },
                },
                true);

            // Assert
            sut.HashAlgorithmsEnabled["MD5"].Should().BeTrue();
            sut.HashAlgorithmsEnabled["SHA-256"].Should().BeTrue();
            sut.HashAlgorithmsEnabled["SHA-512"].Should().BeTrue();
            sut.IsUppercaseHashValues.Should().BeTrue();
        }
    }
}
