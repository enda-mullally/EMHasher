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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EM.Hasher.Services.Activation;

public class ActivationService(
    MainWindow mainWindow,
    IEnumerable<IActivationHandler> activationHandlers,
    DefaultActivationHandler defaultActivationHandler) : IActivationService
{
    public async Task ActivateAsync(object activationArgs)
    {
        App.MainWindow = mainWindow;

        App.MainWindow!.Activate();

        await HandleActivationAsync(activationArgs);
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var handler = activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (handler is not null)
        {
            await handler.HandleAsync(activationArgs);
        }
        else if (defaultActivationHandler.CanHandle(activationArgs))
        {
            await defaultActivationHandler.HandleAsync(activationArgs);
        }
    }
}
