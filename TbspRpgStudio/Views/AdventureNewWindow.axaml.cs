using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.Views;

public partial class AdventureNewWindow : Window
{
    public AdventureNewWindow()
    {
        InitializeComponent();
        
        WeakReferenceMessenger.Default.Register<AdventureNewWindow, AdventureNewClosedMessage>(this,
            static (w, m) => w.Close(m.AdventureView));
    }
}