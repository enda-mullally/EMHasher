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

namespace EM.Hasher.Services.Activation;

/// <summary>
/// Represents a handler capable of processing a specific mode of application
/// activation (e.g. a normal launch or an emhasher:// protocol activation).
/// </summary>
public interface IActivationHandler
{
    /// <summary>
    /// Returns true when this handler is able to process the supplied
    /// activation arguments.
    /// </summary>
    bool CanHandle(object activationArgs);

    /// <summary>
    /// Processes the supplied activation arguments.
    /// </summary>
    Task HandleAsync(object activationArgs);
}
