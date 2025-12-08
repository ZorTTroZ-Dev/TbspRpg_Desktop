using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureViewModel : ViewModelBase
{
    private int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public static async Task<AdventureViewModel> FromAdventure(Adventure adventure)
    {
        // load the description
        var sourceService = TbspRpgDataServiceFactory.Load().SourcesService;
        var description = await sourceService.GetSourceTextForKey(adventure.DescriptionSourceKey);
        return new AdventureViewModel()
        {
            Id = adventure.Id,
            Name =  adventure.Name,
            Description = description
        };
    }

    public static Adventure ToAdventure(AdventureViewModel adventureViewModel)
    {
        return new Adventure()
        {
            Name = adventureViewModel.Name,
            DescriptionSourceKey = Guid.Empty,
            InitialSourceKey =  Guid.Empty,
            InitializationScriptId = null,
            TerminationScriptId = null
        };
    }
    
    [RelayCommand]
    private void EditAdventure()
    {
        // needs to send a message to change the page captured by main window
        WeakReferenceMessenger.Default.Send(new ChangeWindowMessage(new AdventureEditViewModel(Id)));
    }
}