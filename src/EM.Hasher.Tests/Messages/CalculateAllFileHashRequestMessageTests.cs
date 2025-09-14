using EM.Hasher.Messages;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Messages
{
    [TestClass]
    public class CalculateAllFileHashRequestMessageTests
    {
        [TestMethod]
        public void CalculateAllFileHashRequestMessageTests_Works()
        {
            // Arrange
            var fileName = "test.txt";

            // Act
            var sut = new CalculateAllFileHashRequestMessage(fileName, true);

            // Assert
            sut.FileName.Should().Be(fileName);
            sut.OnlyCalculateIfNeeded.Should().BeTrue();
        }
    }
}
