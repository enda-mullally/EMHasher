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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Helpers;
using EM.Hasher.Messages.UI;
using EM.Hasher.Services.Application;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EM.Hasher.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly IAppVersion _appVersion;

    [ObservableProperty]
    public partial string? AppSubTitle { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial Uri? FeedbackUri { get; private set; }

    public ShellViewModel(IAppVersion appVersion)
    {
        _appVersion = appVersion;

        GetFeedbackUri();

        WeakReferenceMessenger.Default.Register<SetAppSubTitleMessage>(this, (r, m) =>
        {
            AppSubTitle = FileNameShortener.ShortenFilename(m.AppSubTitle, 60);
        });
    }

    private void GetFeedbackUri()
    {
        var version = _appVersion.GetVersionDescription();
        var subject = $"EM Hasher Feedback ({version})";
        var urlEncodedSubject = UrlEncoder.Create().Encode(subject);
        var newLines = $"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}";
        var body = $"What do you like?{newLines}What would you like to see next?{newLines}";
        var urlEncodedBody = UrlEncoder.Create().Encode(body);

        FeedbackUri = new Uri($"mailto:em.apps@outlook.ie?subject={urlEncodedSubject}&body={urlEncodedBody}");
    }

    [RelayCommand]
    private async Task DoFeedback()
    {
        await Windows.System.Launcher.LaunchUriAsync(FeedbackUri!);
    }
}
