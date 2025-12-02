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

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EM.Hasher.Controls
{
    public sealed partial class FileInformationControl : UserControl
    {
        public FileInformationControl()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, ForceHardWrap(value));
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register(nameof(FileName), typeof(string), typeof(FileInformationControl), new PropertyMetadata(""));

        public string FileSize
        {
            get => (string)GetValue(FileSizeProperty);
            set => SetValue(FileSizeProperty, value);
        }

        public static readonly DependencyProperty FileSizeProperty =
            DependencyProperty.Register(nameof(FileSize), typeof(string), typeof(FileInformationControl), new PropertyMetadata(""));

        public string FileCreated
        {
            get => (string)GetValue(FileCreatedProperty);
            set => SetValue(FileCreatedProperty, value);
        }

        public static readonly DependencyProperty FileCreatedProperty =
            DependencyProperty.Register(nameof(FileCreated), typeof(string), typeof(FileInformationControl), new PropertyMetadata(""));

        public string FileModified
        {
            get => (string)GetValue(FileModifiedProperty);
            set => SetValue(FileModifiedProperty, value);
        }

        public static readonly DependencyProperty FileModifiedProperty =
            DependencyProperty.Register(nameof(FileModified), typeof(string), typeof(FileInformationControl), new PropertyMetadata(""));

        // This fix's a small issue with wrapping of filename
        // strings with hyphens "-" being treated as line breaks.
        private static string ForceHardWrap(string input)
        {
            return string.Join("\u200B", input.ToCharArray()); // inserts zero-width space between every char
        }
    }
}
