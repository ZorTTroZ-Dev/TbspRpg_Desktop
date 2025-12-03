using Avalonia.Controls;
using Avalonia.Media;

namespace TbspRpgStudio.Views;

public partial class CopyEditSelectView : UserControl
{
    public CopyEditSelectView()
    {
        InitializeComponent();
        var appState = ApplicationState.Load();
        SaveButton.Background = new SolidColorBrush(appState.AccentColor);
    }
}