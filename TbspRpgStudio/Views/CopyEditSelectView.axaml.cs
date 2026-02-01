using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using TbspRpgStudio.ViewModels;

namespace TbspRpgStudio.Views;

public partial class CopyEditSelectView : UserControl
{
    public CopyEditSelectView()
    {
        InitializeComponent();
        var appState = ApplicationState.Load();
        SaveButton.Background = new SolidColorBrush(appState.AccentColor);
    }

    private async void CopyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard == null) return;
        var viewModel = DataContext as CopyEditSelectViewModel;
        await clipboard.SetTextAsync(viewModel?.SelectedCopy?.Key.ToString());
    }
}