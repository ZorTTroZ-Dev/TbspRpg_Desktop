using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgStudio.Messages;
using TbspRpgStudio.Models;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureNewViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _name;

    [RelayCommand (CanExecute = nameof(CanSaveAdventure))]
    public void SaveAdventure()
    {
        var newAdventureView = new AdventureView()
        {
            Name = Name,
            Description = "Viva la France!"
        };
        WeakReferenceMessenger.Default.Send(new AdventureNewClosedMessage(newAdventureView));
    }

    private bool CanSaveAdventure()
    {
        return Name != null;
    }
    
    [RelayCommand]
    public void CancelAdventure()
    {
        WeakReferenceMessenger.Default.Send(new AdventureNewClosedMessage(null));
    }
}