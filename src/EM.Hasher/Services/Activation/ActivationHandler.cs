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
/// Base class for a strongly-typed activation handler. Handlers only run when
/// the activation arguments match the expected type <typeparamref name="T"/>.
/// </summary>
public abstract class ActivationHandler<T> : IActivationHandler where T : class
{
    protected abstract bool CanHandleInternal(T args);

    protected virtual ActivationVerificationResult VerifyInternal(T args) =>
        ActivationVerificationResult.Valid;

    protected abstract Task HandleInternalAsync(T args);

    public bool CanHandle(object activationArgs) =>
        activationArgs is T args && CanHandleInternal(args);

    public ActivationVerificationResult Verify(object activationArgs) =>
        VerifyInternal((T)activationArgs);

    public Task HandleAsync(object activationArgs) =>
        HandleInternalAsync((T)activationArgs);
}
