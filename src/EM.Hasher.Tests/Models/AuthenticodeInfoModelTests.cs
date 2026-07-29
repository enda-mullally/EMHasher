using EM.Hasher.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Models;

[TestClass]
public class AuthenticodeInfoModelTests
{
    [TestMethod]
    public void AuthenticodeInfoModel_Works()
    {
        // Arrange
        var isSigned = true;
        var isTrusted = true;
        var signer = "Contoso Ltd";
        var issuer = "Contoso Root CA";
        var isTimeStamped = true;
        var signingTime = "Monday, 1 January 2024 10:00 (2 years ago)";

        // Act
        var sut = new AuthenticodeInfoModel
        {
            IsSigned = isSigned,
            IsTrusted = isTrusted,
            Signer = signer,
            Issuer = issuer,
            IsTimeStamped = isTimeStamped,
            SigningTime = signingTime
        };

        // Assert
        sut.IsSigned.Should().Be(isSigned);
        sut.IsTrusted.Should().Be(isTrusted);
        sut.Signer.Should().Be(signer);
        sut.Issuer.Should().Be(issuer);
        sut.IsTimeStamped.Should().Be(isTimeStamped);
        sut.SigningTime.Should().Be(signingTime);
    }

    [TestMethod]
    public void AuthenticodeInfoModel_Defaults_AreExpected()
    {
        // Act
        var sut = new AuthenticodeInfoModel();

        // Assert
        sut.IsSigned.Should().BeFalse();
        sut.IsTrusted.Should().BeFalse();
        sut.Signer.Should().BeEmpty();
        sut.Issuer.Should().BeEmpty();
        sut.IsTimeStamped.Should().BeFalse();
        sut.SigningTime.Should().BeEmpty();
    }
}
