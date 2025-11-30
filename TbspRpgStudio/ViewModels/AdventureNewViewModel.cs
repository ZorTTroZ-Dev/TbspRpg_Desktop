using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureNewViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _name;

    [RelayCommand (CanExecute = nameof(CanSaveAdventure))]
    public void SaveAdventure()
    {
        var newAdventure = new Adventure()
        {
            Name = Name,
            DescriptionSourceKey = Guid.Empty,
            InitialSourceKey =  Guid.Empty,
            InitializationScriptId = null,
            TerminationScriptId = null
        };
        WeakReferenceMessenger.Default.Send(new AdventureNewClosedMessage(newAdventure));
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