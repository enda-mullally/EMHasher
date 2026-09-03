using EM.Hasher.Messages;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Messages;

[TestClass]
public class IsUiBusyMessageTests
{
    [TestMethod]
    public void IsUiBusyMessage_Works()
    {
        // Arrange
        var isBusy = true;

        // Act
        var sut = new IsUiBusyMessage(isBusy);

        // Assert
        sut.IsBusy.Should().Be(isBusy);
    }
}
