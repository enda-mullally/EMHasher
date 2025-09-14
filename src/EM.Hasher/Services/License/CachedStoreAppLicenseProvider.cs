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

using EM.Hasher.Helpers;
using EM.Hasher.Models;
using System;
using System.Threading.Tasks;

namespace EM.Hasher.Services.License
{
    public class CachedStoreAppLicenseProvider(/*IEventLogWriter eventLog*/) : ICachedStoreAppLicense
    {
//        private readonly IEventLogWriter _eventLog = eventLog;

        private StoreAppLicenseModel? _cachedLicense;

        public async Task<StoreAppLicenseModel?> GetCachedStoreAppLicenseAsync()
        {
            if (_cachedLicense == null)
            {
                _cachedLicense = await GetStoreAppLicenseAsync()
                    .TimeoutAfter(TimeSpan.FromSeconds(5));
            }

            return _cachedLicense;
        }

        private async Task<StoreAppLicenseModel> GetStoreAppLicenseAsync()
        {
/*
#if RELEASE
            try
            {
                _eventLog.WriteInfo("Checking store license");

                Windows.Services.Store.StoreContext context = Windows.Services.Store.StoreContext.GetDefault();
                Windows.Services.Store.StoreAppLicense appLicense = await context.GetAppLicenseAsync();

                _eventLog.WriteInfo("Checking store license complete");

                var result = new StoreAppLicenseModel()
                {
                    IsActive = appLicense.IsActive,
                    IsTrial = appLicense.IsTrial,
                    ExpirationDate = appLicense.ExpirationDate,
                    Data= appLicense.ExtendedJsonData
                };

                _eventLog.WriteInfo("License details: " + result.ToString());

                return result;
            }
            catch (Exception ex)
            {
                _eventLog.WriteInfo("Store license check failed or timed out" + ex.Message);

                // Some sort of error occurred,
                // default to trial license expired page.
                var result = new StoreAppLicenseModel()
                {
                    IsActive = false,
                    IsTrial = true,
                    ExpirationDate = new DateTimeOffset(DateTime.Now.AddDays(-5))
                };

                _eventLog.WriteInfo("License details: " + result.ToString());

                return result;
            }
#else
*/
            //_eventLog.WriteInfo("Debug license");

            // Note: As of version 1.1.58 the app is now free and unrestricted.

            return await Task.FromResult(
                new StoreAppLicenseModel()
                {
                    IsActive = true,     // set to false to expire
                    IsTrial = false,     // set to false to indicate full version (no trial)

                    // only applies when in trial mode
                    ExpirationDate = DateTime.MinValue
                });
/*
#endif
*/
        }
    }
}