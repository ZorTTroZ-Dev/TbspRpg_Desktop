using System;
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
        // TODO write the created adventure to the database
        // TODO send a notification message to let the user know how creation went
    }
}