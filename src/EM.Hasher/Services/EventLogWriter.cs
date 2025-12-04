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
using System.Diagnostics;

namespace EM.Hasher.Services
{
    public class EventLogWriter : IEventLogWriter
    {
        private readonly EventLog? _eventLog;
        private readonly bool _loggingEnabled;
        private readonly string _logPrefix;

        public EventLogWriter(
            string logName,
            string appSourceName,
            string loggingEnvVarName)
        {
            _loggingEnabled =
                Environment.GetEnvironmentVariable(loggingEnvVarName) != null &&
                (Environment.GetEnvironmentVariable(loggingEnvVarName)!.Equals("true", StringComparison.InvariantCultureIgnoreCase) ||
                 Environment.GetEnvironmentVariable(loggingEnvVarName)!.Equals("1", StringComparison.InvariantCultureIgnoreCase));

            if (!_loggingEnabled)
            {
                _logPrefix = string.Empty;

                return;
            }

            var sourceExists = AppSourceExists(appSourceName);
            var sn = sourceExists ? appSourceName : "Application";

            if (sn.Equals("Application", StringComparison.InvariantCultureIgnoreCase))
            {
                _logPrefix = $"[{appSourceName}] ";
                _eventLog = new EventLog("Application");
            }
            else
            {
                _logPrefix = string.Empty;
                _eventLog = new EventLog($"{logName}");
            }

            _eventLog.Source = sn;
        }

        private static bool AppSourceExists(string sourceName)
        {
            var initMessage = "EM Hasher App Init";

            try
            {
                // Try writing to the custom source directly
                EventLog.WriteEntry(sourceName, initMessage, EventLogEntryType.Information);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void WriteInfo(string info)
        {
            if (!_loggingEnabled || _eventLog == null)
            {
                return;
            }

            _eventLog.WriteEntry(_logPrefix + info, EventLogEntryType.Information);
        }

        public void WriteError(string error)
        {
            if (!_loggingEnabled || _eventLog == null)
            {
                return;
            }

            _eventLog.WriteEntry(_logPrefix + error, EventLogEntryType.Error);
        }
    }
}
