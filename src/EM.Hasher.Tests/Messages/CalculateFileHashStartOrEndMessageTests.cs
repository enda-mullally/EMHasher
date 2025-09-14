using EM.Hasher.Messages;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Messages
{
    [TestClass]
    public class CalculateFileHashStartOrEndMessageTests
    {
        [TestMethod]
        public void CalculateFileHashStartOrEndMessage_Works()
        {
            // Arrange
            var algorithmName = "MD5";

            // Act
            var sut = new CalculateFileHashStartOrEndMessage(algorithmName, true);

            // Assert
            sut.AlgorithmName.Should().Be(algorithmName);
            sut.IsStart.Should().BeTrue();
        }
    }
}
