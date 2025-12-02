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

using System;
using System.Collections.Generic;
using EM.Hasher.Pages;
using EM.Hasher.Services;
using EM.Hasher.Services.Application;
using EM.Hasher.Services.File;
using EM.Hasher.Services.Hashes;
using EM.Hasher.Services.License;
using EM.Hasher.Services.Navigation;
using EM.Hasher.Services.Settings;
using EM.Hasher.ViewModels;
using EM.Hasher.ViewModels.Controls;
using EM.Hasher.ViewModels.Pages;
using EM.Hasher.ViewModels.UI;
using Microsoft.Extensions.DependencyInjection;

namespace EM.Hasher.DI
{
    public partial class Container
    {
        private Container RegisterServices()
        {
            // Singleton services
            _container.AddSingleton<ISettingsProvider, SettingsProvider>();
            _container.AddSingleton<INavigationService, NavigationService>();
            _container.AddSingleton<ICachedStoreAppLicense, CachedStoreAppLicenseProvider>();

            // Views
            _container.AddSingleton<Shell>();               // Main frame content, used to host all other pages
            _container.AddSingleton<TrialExpired>();        // Trial ezxpired frame content

            // Views ViewModels
            _container.AddSingleton<UIStateViewModel>();    // Used to manage the enabled/disabled state of some UI elements 
            _container.AddSingleton<ShellViewModel>();
            _container.AddSingleton<HomeViewModel>();
            _container.AddSingleton<CalculateViewModel>();
            _container.AddSingleton<SettingsViewModel>();

            // Services
            _container.AddTransient<IAppVersion, AppVersion>();
            _container.AddTransient<IFileDetailsProvider, FileDetailsProvider>();

            _container.AddSingleton(CreateFileHashControlViewModels);
            _container.AddSingleton<IEventLogWriter, EventLogWriter>(CreateEventLogWriter);

            return this;
        }

        private static IList<FileHashControlViewModel> CreateFileHashControlViewModels(IServiceProvider provider)
        {
            var settings = provider
                .GetRequiredService<ISettingsProvider>();

            return
            [
                new FileHashControlViewModel(
                    new Crc32HashCalculator(),
                    settings.IsUppercaseHashValues,
                    settings.IsCrc32Enabled),

                new FileHashControlViewModel(
                    new Md5HashCalculator(),
                    settings.IsUppercaseHashValues,
                    settings.IsMd5Enabled),

                new FileHashControlViewModel(
                    new Sha256HashCalculator(),
                    settings.IsUppercaseHashValues,
                    settings.IsSha256Enabled),

                new FileHashControlViewModel(
                    new Sha512HashCalculator(),
                    settings.IsUppercaseHashValues,
                    settings.IsSha512Enabled)
            ];
        }

        private static EventLogWriter CreateEventLogWriter(IServiceProvider provider)
        {
            return new EventLogWriter("Enda Mullally", "EM Hasher", "EM_HASHER_LOG");
        }
    }
}
