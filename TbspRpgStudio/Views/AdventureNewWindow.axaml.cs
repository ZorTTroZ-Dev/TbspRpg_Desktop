using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.Views;

public partial class AdventureNewWindow : Window
{
    public AdventureNewWindow()
    {
        InitializeComponent();
        
        WeakReferenceMessenger.Default.Register<AdventureNewWindow, AdventureNewClosedMessage>(this,
            static (w, m) => w.Close(m.AdventureViewModel));
        
        var appState = ApplicationState.Load();
        ContentBorder.BorderBrush = new SolidColorBrush(appState.AlternateColor);
        SaveButton.Background =  new SolidColorBrush(appState.AccentColor);
    }
}