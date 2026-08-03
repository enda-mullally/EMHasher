using System.Threading.Tasks;
using EM.Hasher.Services.Verification;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Verification;

[TestClass]
public class DummyHashVerificationServiceTests
{
    private static DummyHashVerificationService CreateSut()
    {
        return new DummyHashVerificationService();
    }

    [TestMethod]
    public async Task VerifyAsync_AlwaysReturnsNoVerificationHashFoundAsync()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.VerifyAsync(@"C:\temp\test.bin", "abc123");

        // Assert
        result.Should().NotBeNull();
        result.VerificationHashFound.Should().BeFalse();
        result.IsHashMatching.Should().BeFalse();
        result.HashVerificationDescription.Should().BeEmpty();
    }
}
