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
using CommunityToolkit.Mvvm.Messaging;
using EM.Hasher.Helpers;
using EM.Hasher.Messages.UI;

namespace EM.Hasher.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? AppSubTitle { get; private set; } = string.Empty;

    public ShellViewModel()
    {
        WeakReferenceMessenger.Default.Register<SetAppSubTitleMessage>(this, (r, m) =>
        {
            AppSubTitle = FileNameShortener.ShortenFilename(m.AppSubTitle, 60);
        });
    }
}
