using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureListViewModel : ViewModelBase
{
    [RelayCommand]
    private async Task AddAdventureAsync()
    {
        var adventure = await WeakReferenceMessenger.Default.Send(new AdventureNewMessage());

        try
        {
            var dataLayer = TbspRpgDataLayer.TbspRpgDataLayer.Load();
            await dataLayer.AdventuresService.AddAdventure(adventure);
            await dataLayer.AdventuresService.SaveChanges();
        }
        catch (Exception e)
        {
            WeakReferenceMessenger.Default.Send(new NotificationMessage(e.Message, NotificationType.Error));
        }

        WeakReferenceMessenger.Default.Send(new NotificationMessage(
            $"Adventure {adventure?.Name} was successfully added", NotificationType.Success));
    }
}