using EM.Hasher.Messages.UI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Messages.UI
{
    [TestClass]
    public class HomeFileSelectedMessageTests
    {
        [TestMethod]
        public void HomeFileSelectedMessage_Works()
        {
            // Act
            var sut = new HomeFileSelectedMessage(true);

            // Assert
            sut.IsFileSelected.Should().BeTrue();
        }
    }
}
