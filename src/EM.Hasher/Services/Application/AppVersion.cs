using EM.Hasher.Helpers;
using System.Reflection;
using Windows.ApplicationModel;

namespace EM.Hasher.Services.Application
{
    public class AppVersion : IAppVersion
    {
        public string GetVersionDescription()
        {
            System.Version version;

            if (RuntimeHelper.IsMSIX)
            {
                PackageVersion packageVersion = Package.Current.Id.Version;

                version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
            }
            else
            {
                version = Assembly.GetExecutingAssembly().GetName().Version!;
            }

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
