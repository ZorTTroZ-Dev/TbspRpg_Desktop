using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace TbspRpgStudio.Views;

public partial class CopyEditCreateView : UserControl
{
    public CopyEditCreateView()
    {
        InitializeComponent();
        var appState = ApplicationState.Load();
        SaveButton.Background = new SolidColorBrush(appState.AccentColor);
        LayoutUpdated += OnLayoutUpdated;
    }
    
    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        PreviewBorder.BorderBrush = TextBox.BorderBrush;
        PreviewBorder.Background = TextBox.Background;
    }
}