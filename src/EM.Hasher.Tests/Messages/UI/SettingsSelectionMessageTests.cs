using EM.Hasher.Messages.UI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Messages.UI
{
    [TestClass]
    public class SettingsSelectionMessageTests
    {
        [TestMethod]
        public void SettingsSelectionMessage_Works()
        {
            // Act
            var sut = new SettingsSelectionMessage(true);

            // Assert
            sut.IsSettingsSelectionValid.Should().BeTrue();
        }
    }
}
