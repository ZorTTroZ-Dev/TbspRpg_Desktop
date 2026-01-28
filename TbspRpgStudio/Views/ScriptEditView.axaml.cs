using Avalonia.Controls;
using Avalonia.Media;

namespace TbspRpgStudio.Views;

public partial class ScriptEditView : UserControl
{
    public ScriptEditView()
    {
        InitializeComponent();
        var appState = ApplicationState.Load();
        SaveButton.Background = new SolidColorBrush(appState.AccentColor);
        SaveCloseButton.Background = new SolidColorBrush(appState.AccentColor);
    }
}