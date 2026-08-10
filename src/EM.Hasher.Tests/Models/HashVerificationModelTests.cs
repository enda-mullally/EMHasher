using EM.Hasher.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Models;

[TestClass]
public class HashVerificationModelTests
{
    [TestMethod]
    public void HashVerificationModel_Defaults_AreExpected()
    {
        // Act
        var sut = new HashVerificationModel();

        // Assert
        sut.VerificationHashFound.Should().BeFalse();
        sut.IsHashMatching.Should().BeFalse();
        sut.HashVerificationDescription.Should().BeEmpty();
    }

    [TestMethod]
    public void HashVerificationModel_Works()
    {
        // Arrange
        var description = "Verification passed. Matching hash found in 'test.sha256'.";

        // Act
        var sut = new HashVerificationModel
        {
            VerificationHashFound = true,
            IsHashMatching = true,
            HashVerificationDescription = description
        };

        // Assert
        sut.VerificationHashFound.Should().BeTrue();
        sut.IsHashMatching.Should().BeTrue();
        sut.HashVerificationDescription.Should().Be(description);
    }
}
