using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgStudio.Messages;
using TbspRpgStudio.Models;
using TbspRpgStudio.ViewModels;

namespace TbspRpgStudio.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
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
            m.Reply(dialog.ShowDialog<AdventureView?>(w));
        });
        
        WeakReferenceMessenger.Default.Register<MainWindow, NotificationMessage>(this, static (w, m) =>
        {
            w.NotificationManager.CloseAll();
            w.NotificationManager.Show(m.Message, m.Type, TimeSpan.FromSeconds(5));
        });
    }
}