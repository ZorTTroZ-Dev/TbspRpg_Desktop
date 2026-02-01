using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using TbspRpgStudio.ViewModels;

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

    private async void CopyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard == null) return;
        var viewModel = DataContext as CopyEditCreateViewModel;
        if  (viewModel == null) return;
        await viewModel.SaveChanges();
        await clipboard.SetTextAsync(viewModel.Copy.Key.ToString());
    }
}