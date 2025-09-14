using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using Windows.ApplicationModel;
using WinUIEx;

namespace EM.Hasher
{
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            InitializeComponent();

            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            
            var assetsFolderIconPath = Path.Combine(FindAssetsFolder(), "EMHasher.ico");
            AppWindow.SetIcon(assetsFolderIconPath);
            AppWindow.SetTaskbarIcon(assetsFolderIconPath);

            this.CenterOnScreen();
        }

        private static string FindAssetsFolder()
        {
            // The assets folder is normally located in the same directory as the executable.
            // However this packaged and deployed locally app may have a different structure, we may need
            // traverse to the parent folder to see the correct assets folder.
            string assetsFolder = Path.Combine(AppContext.BaseDirectory, "Assets");
            if (Directory.Exists(assetsFolder))
            {
                return assetsFolder;
            }

            assetsFolder = Path.Combine(Package.Current.InstalledLocation.Path, "Assets");
            if (Directory.Exists(assetsFolder))
            {
                return assetsFolder;
            }

            var dirInfo = new DirectoryInfo(Package.Current.InstalledLocation.Path);
            var parent = dirInfo.Parent;
            assetsFolder = Path.Combine(parent!.FullName, "Assets");
            if (Directory.Exists(assetsFolder))
            {
                return assetsFolder;
            }

            return assetsFolder;
        }

        // Based on https://github.com/microsoft/devhome/commit/26e01a7fc8ec965c365b275a77adb4e7522317f1
        // Occassionally I've seen exceptions/crashes when closing the application, as per above DevHome
        // commit doing the same here.
        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            // WinUI bug is causing a crash on shutdown when FailFastOnErrors is set to true (#51773592).
            // Workaround by turning it off before shutdown.
            App.Current.DebugSettings.FailFastOnErrors = false;
        }
    }
}
