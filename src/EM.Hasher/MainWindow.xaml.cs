/*
 * EM Hasher
 * Copyright © 2025 Enda Mullally (em.apps@outlook.ie)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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
