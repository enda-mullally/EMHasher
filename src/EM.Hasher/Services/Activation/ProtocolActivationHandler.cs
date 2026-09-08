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
using System.Threading.Tasks;
using EM.Hasher.ViewModels;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;

namespace EM.Hasher.Services.Activation;

/// <summary>
/// Handles the "hash" verb of the emhasher:// protocol (the "Hash with EM
/// Hasher" context-menu launch).
/// Expected form: emhasher://hash?file=<url-encoded-path>
/// </summary>
public class ProtocolActivationHandler(HomeViewModel homeViewModel) : ActivationHandler<AppActivationArguments>
{
    private const string HashVerb = "hash";

    protected override bool CanHandleInternal(AppActivationArguments args) =>
        args.Kind == ExtendedActivationKind.Protocol &&
        args.Data is IProtocolActivatedEventArgs protocolArgs &&
        string.Equals(ParseVerbFromUri(protocolArgs.Uri), HashVerb, StringComparison.OrdinalIgnoreCase);

    protected override ActivationVerificationResult VerifyInternal(AppActivationArguments args)
    {
        var protocolArgs = (IProtocolActivatedEventArgs)args.Data;

        var filePath = ParseFileFromUri(protocolArgs.Uri);

        if (!string.IsNullOrWhiteSpace(filePath) && !System.IO.File.Exists(filePath))
        {
            return ActivationVerificationResult.Invalid(
                "The file you provided could not be found.");
        }

        return ActivationVerificationResult.Valid;
    }

    protected override Task HandleInternalAsync(AppActivationArguments args)
    {
        var protocolArgs = (IProtocolActivatedEventArgs)args.Data;

        var filePath = ParseFileFromUri(protocolArgs.Uri);

        if (!string.IsNullOrWhiteSpace(filePath))
        {
            if (App.IsShellReady)
            {
                // The Shell (and its NavigationView) is already loaded, so
                // route the file through the normal Calculate flow now.
                _ = homeViewModel.SelectFileAsync(filePath!);
            }
            else
            {
                // Cold start: defer until the Shell has performed its default
                // navigation, otherwise it would override us back to Home.
                App.PendingActivationFilePath = filePath;
            }
        }

        return Task.CompletedTask;
    }

    private static string? ParseVerbFromUri(Uri uri)
    {
        return uri.Host;
    }

    private static string? ParseFileFromUri(Uri uri)
    {
        try
        {
            var decoder = new WwwFormUrlDecoder(uri.Query);
            return decoder.GetFirstValueByName("file");
        }
        catch
        {
            return null;
        }
    }
}
