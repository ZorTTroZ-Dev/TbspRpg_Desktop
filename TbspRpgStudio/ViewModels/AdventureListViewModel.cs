using System.Threading.Tasks;
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

        // write to database
        var dataLayer = TbspRpgDataLayer.TbspRpgDataLayer.Load();
        await dataLayer.AdventuresService.AddAdventure(adventure);
        await dataLayer.AdventuresService.SaveChanges();
        
        WeakReferenceMessenger.Default.Send(new NotificationMessage($"Adventure {adventure?.Name} was successfully added"));
    }
}