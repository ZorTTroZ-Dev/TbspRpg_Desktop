using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgProcessor;
using TbspRpgProcessor.Entities;
using TbspRpgSettings.Settings;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureListViewModel : ViewModelBase
{
    public ObservableCollection<AdventureViewModel> Adventures { get; } = new();
    private readonly TbspRpgDataServiceFactory _dataServiceFactory;

    public AdventureListViewModel()
    {
        _dataServiceFactory = TbspRpgDataServiceFactory.Load();
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        Adventures.Clear();
        var adventures = await _dataServiceFactory.AdventuresService.GetAllAdventures(new AdventureFilter());
        foreach (var adventure in adventures)
        {
            Adventures.Add(await AdventureViewModel.FromAdventure(adventure));
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
            var appState = ApplicationState.Load();
            await TbspRpgProcessorFactory.TbspRpgProcessor().CreateAdventureInitial(new AdventureCreateModel()
            {
                Name = adventureView.Name,
                Description = adventureView.Description,
                Languages = adventureView.Languages,
                DescriptionLanguage = appState.Language
            });
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