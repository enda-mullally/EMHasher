using EM.Hasher.Messages;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Messages
{
    [TestClass]
    public class FilePickedMessageTests
    {
        [TestMethod]
        public void FilePickedMessage_Works()
        {
            // Arrange
            var fileName = "test.txt";

            // Act
            var sut = new FilePickedMessage
            {
                FileName = fileName,
                ItsNew = true
            };

            // Assert
            sut.FileName.Should().Be(fileName);
            sut.ItsNew.Should().BeTrue();
        }
    }
}
