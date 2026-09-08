/*
 * EM Hasher
 * Copyright © 2026 Enda Mullally (em.apps@outlook.ie)
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace EM.Hasher.Services.Activation;

public class ActivationService(
    MainWindow mainWindow,
    IEnumerable<IActivationHandler> activationHandlers) : IActivationService
{
    private const uint MB_OK = 0x00000000;
    private const uint MB_ICONERROR = 0x00000010;

    public async Task ActivateAsync(object activationArgs)
    {
        App.MainWindow = mainWindow;

        var handler = activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (handler is not null)
        {
            var verification = handler.Verify(activationArgs);

            if (!verification.IsValid)
            {
                MessageBox(IntPtr.Zero,
                        verification.ErrorMessage!,
                        "EM Hasher",
                        MB_OK | MB_ICONERROR);

                Microsoft.UI.Xaml.Application.Current.Exit();

                return;
            }

            await handler.HandleAsync(activationArgs);
        }

        // Activation is valid, so show the main window.
        App.MainWindow!.Activate();
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MessageBoxW")]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
}
