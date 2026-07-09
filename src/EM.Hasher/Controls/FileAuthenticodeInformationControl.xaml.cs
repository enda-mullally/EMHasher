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

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EM.Hasher.Controls;

public sealed partial class FileAuthenticodeInformationControl : UserControl
{
    public FileAuthenticodeInformationControl()
    {
        InitializeComponent();
    }

    public string Signer
    {
        get => (string)GetValue(SignerProperty);
        set => SetValue(SignerProperty, value);
    }

    public static readonly DependencyProperty SignerProperty =
        DependencyProperty.Register(nameof(Signer), typeof(string), typeof(FileAuthenticodeInformationControl), new PropertyMetadata(""));

    public string Issuer
    {
        get => (string)GetValue(IssuerProperty);
        set => SetValue(IssuerProperty, value);
    }

    public static readonly DependencyProperty IssuerProperty =
        DependencyProperty.Register(nameof(Issuer), typeof(string), typeof(FileAuthenticodeInformationControl), new PropertyMetadata(""));

    public bool IsTimeStamped
    {
        get => (bool)GetValue(IsTimeStampedProperty);
        set => SetValue(IsTimeStampedProperty, value);
    }

    public static readonly DependencyProperty IsTimeStampedProperty =
        DependencyProperty.Register(nameof(IsTimeStamped), typeof(bool), typeof(FileAuthenticodeInformationControl), new PropertyMetadata(false));

    public string SigningTime
    {
        get => (string)GetValue(SigningTimeProperty);
        set => SetValue(SigningTimeProperty, value);
    }

    public static readonly DependencyProperty SigningTimeProperty =
        DependencyProperty.Register(nameof(SigningTime), typeof(string), typeof(FileAuthenticodeInformationControl), new PropertyMetadata(""));
}
