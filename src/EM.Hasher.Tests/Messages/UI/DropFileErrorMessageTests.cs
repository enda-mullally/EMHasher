using EM.Hasher.Messages.UI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Messages.UI
{
    [TestClass]
    public class DropFileErrorMessageTests
    {
        [TestMethod]
        public void DropFileErrorMessage_Works()
        {
            // Act
            var sut = new DropFileErrorMessage(true, "Oh no!");

            // Assert
            sut.IsDropFileError.Should().BeTrue();
            sut.ErrorMessage.Should().BeEquivalentTo("Oh no!");
        }
    }
}
