using EM.Hasher.Models;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EM.Hasher.Tests.Models
{
    [TestClass]
    public class StoreAppLicenseModelTests
    {
        [TestMethod]
        public void StoreAppLicenseModel_Works()
        {
            // Arrange
            var testDate = DateTime.UtcNow.ToDateTimeOffset();

            // Act
            var sut = new StoreAppLicenseModel()
            {
                ExpirationDate = testDate,
                IsActive = true,
                IsTrial = true,
                Data = "TestData"
            };

            // Assert
            sut.ExpirationDate.Should().Be(testDate);
            sut.IsActive.Should().BeTrue();
            sut.IsTrial.Should().BeTrue();
            sut.ToString().Should().Be("IsActive=True, IsTrial=True, ExpirationDate=" + testDate.ToString() + ", Data=TestData");
        }
    }
}
