using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgStudio.Messages;
using TbspRpgStudio.Models;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureListViewModel : ViewModelBase
{
    public ObservableCollection<AdventureView> Adventures { get; } = new();
    private readonly TbspRpgDataLayer.TbspRpgDataLayer _dataLayer;

    public AdventureListViewModel()
    {
        _dataLayer = TbspRpgDataLayer.TbspRpgDataLayer.Load();
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        Adventures.Clear();
        var adventures = await _dataLayer.AdventuresService.GetAllAdventures(new AdventureFilter());
        foreach (var adventure in adventures)
        {
            Adventures.Add(AdventureView.FromAdventure(adventure));
        }
    }
    
    [RelayCommand]
    private async Task AddAdventureAsync()
    {
        var adventureView = await WeakReferenceMessenger.Default.Send(new AdventureNewMessage());

        if (adventureView == null)
            return;

        try
        {
            await _dataLayer.AdventuresService.AddAdventure(AdventureView.ToAdventure(adventureView));
            await _dataLayer.AdventuresService.SaveChanges();
            await LoadDataAsync();
        }
        catch (Exception e)
        {
            WeakReferenceMessenger.Default.Send(new NotificationMessage(e.Message, NotificationType.Error));
        }

        WeakReferenceMessenger.Default.Send(new NotificationMessage(
            $"Adventure {adventureView.Name} was successfully added", NotificationType.Success));
    }
}