// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Tweaked version of the CopyButton control from the Windows Community Toolkit
// https://github.com/CommunityToolkit/WindowsCommunityToolkit
// Modified to work with AOT & trimming by Enda Mullally 2025

using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace EM.Hasher.Controls;

public sealed partial class CopyButton : Button
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ResourceDictionary))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Style))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Storyboard))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Grid))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DoubleAnimationUsingKeyFrames))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DiscreteDoubleKeyFrame))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SplineDoubleKeyFrame))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(LinearDoubleKeyFrame))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ContentPresenter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(VisualStateManager))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(VisualStateGroup))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(VisualState))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ObjectAnimationUsingKeyFrames))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ControlTemplate))]
    public CopyButton()
    {
        DefaultStyleKey = typeof(CopyButton);
        Content = new FontIcon
        {
            Glyph = "\uE8C8", // Automatically set a copy glyph
            FontFamily = new("Segoe MDL2 Assets"),
            FontSize = 16
        };
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Template != null)
        {
            if (button is CopyButton copyButton)
            {
                var rootGrid = copyButton.GetTemplateChild("RootGrid") as Grid;

                if (rootGrid != null &&
                    rootGrid.Resources.TryGetValue("CopyToClipboardSuccessAnimationKey", out var res) &&
                    res is Storyboard storyboard)
                {
                    storyboard.Begin();
                }
            }
        }
    }

    protected override void OnApplyTemplate()
    {
        Click -= CopyButton_Click;
        base.OnApplyTemplate();
        Click += CopyButton_Click;
    }
}
