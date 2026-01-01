using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureViewModel : ViewModelBase
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private string? _name;
    [ObservableProperty] private string? _description;
    [ObservableProperty] private List<Language?>? _languages;

    public static async Task<AdventureViewModel> FromAdventure(Adventure adventure)
    {
        // load the description
        var copyService = TbspRpgDataServiceFactory.Load().CopyService;
        var appState = ApplicationState.Load();
        var description = await copyService.GetCopyTextForKey(adventure.DescriptionSourceKey, appState.Language);
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
    private async Task EditAdventure()
    {
        // needs to send a message to change the page captured by main window
        var adventureEditViewModel = await AdventureEditViewModel.CreateAsync(Id);
        WeakReferenceMessenger.Default.Send(new ChangeWindowMessage(adventureEditViewModel));
    }
}