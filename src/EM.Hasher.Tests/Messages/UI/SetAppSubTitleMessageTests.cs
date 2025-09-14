using EM.Hasher.Messages.UI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Messages.UI
{
    [TestClass]
    public class SetAppSubTitleMessageTests
    {
        [TestMethod]
        public void SetAppSubTitleMessage_Works()
        {
            // Act
            var sut = new SetAppSubTitleMessage("test");

            // Assert
            sut.AppSubTitle.Should().BeEquivalentTo("test");
        }
    }
}
