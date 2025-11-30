using EM.Hasher.Services.Application;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Application
{
    [TestClass]
    public class AppVersionTests
    {
        [TestMethod]
        public void AppVersion_Works()
        {
            var appVersion = new AppVersion();

            // This will be run against the test harness and
            // likely return 1.0.0.0, any version here will be fine

            appVersion.GetVersionDescription().Should().NotBeNullOrEmpty();
        }
    }
}
