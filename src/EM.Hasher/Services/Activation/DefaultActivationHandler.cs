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

using System.Threading.Tasks;
using Microsoft.Windows.AppLifecycle;

namespace EM.Hasher.Services.Activation;

/// <summary>
/// Fallback handler used when no other handler can process the activation
/// (e.g. a normal launch). The main window activation itself is performed by
/// the <see cref="ActivationService"/>, so there is nothing extra to do here.
/// </summary>
public class DefaultActivationHandler : ActivationHandler<AppActivationArguments>
{
    protected override bool CanHandleInternal(AppActivationArguments args) => true;

    protected override Task HandleInternalAsync(AppActivationArguments args) => Task.CompletedTask;
}
