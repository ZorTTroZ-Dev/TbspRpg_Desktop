using Avalonia.Controls;

namespace TbspRpgStudio.Views;

public partial class AdventureEditView : UserControl
{
    private readonly double _mainWidth = 700;
    private readonly double _viewSwitchWidth = 48;
    
    public AdventureEditView()
    {
        InitializeComponent();
        var appState = ApplicationState.Load();
        var panelWidth = appState.WindowWidth * 0.39; // Golden Ratio
        var remainder = appState.WindowWidth - panelWidth;
        var gutterWidth = (remainder - _mainWidth - _viewSwitchWidth) / 2;
        SplitView.OpenPaneLength = panelWidth;
        GridGutter.Width = gutterWidth;
        GridViewSwitch.Width = _viewSwitchWidth;
        GridMainContent.Width = _mainWidth;
    }
}