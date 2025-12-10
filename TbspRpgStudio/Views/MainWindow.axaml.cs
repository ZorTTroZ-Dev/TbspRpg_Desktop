using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgStudio.Messages;
using TbspRpgStudio.ViewModels;
using AdventureViewModel = TbspRpgStudio.ViewModels.AdventureViewModel;

namespace TbspRpgStudio.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Width = 1280;
        // Height = 720;
        
        // The designer cannot open a new window, so we don't register the messenger if we have a design session. 
        if (Design.IsDesignMode)
            return;
        
        // Whenever 'Send(new PurchaseAlbumMessage())' is called, invoke this callback on the MainWindow instance:
        WeakReferenceMessenger.Default.Register<MainWindow, AdventureNewMessage>(this, static (w, m) =>
        {
            // Create an instance of AdventureNewWindow and set AdventureNewViewModel as its DataContext.
            var dialog = new AdventureNewWindow()
            {
                DataContext = new AdventureNewViewModel()
            };
            // TODO: Cap height and width
            dialog.Width = 512;
            dialog.Height = 288;
            // Show dialog window and reply with returned AdventureViewModel or null when the dialog is closed.
            m.Reply(dialog.ShowDialog<AdventureViewModel?>(w));
        });
        
        WeakReferenceMessenger.Default.Register<MainWindow, NotificationMessage>(this, static (w, m) =>
        {
            w.NotificationManager.CloseAll();
            w.NotificationManager.Show(m.Message, m.Type, TimeSpan.FromSeconds(5));
        });
        
        WeakReferenceMessenger.Default.Register<MainWindow, ChangeWindowMessage>(this, (w, m) =>
        {
            if (DataContext is MainWindowViewModel context) context.CurrentPageViewModel = m.ViewModel;
        });

        LayoutUpdated += OnLayoutUpdated;
    }
    
    private void OnLayoutUpdated(object? sender, System.EventArgs e)
    {
        var appState = ApplicationState.Load();
        appState.WindowWidth = Width;
        appState.WindowHeight = Height;
        appState.AccentColor = PlatformSettings.GetColorValues().AccentColor1;
    }
}