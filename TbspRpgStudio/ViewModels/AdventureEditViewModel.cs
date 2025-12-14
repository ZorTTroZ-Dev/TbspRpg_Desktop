using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgSettings.Settings;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureEditViewModel : ViewModelBase
{
    [ObservableProperty] private Adventure? _adventure;
    [ObservableProperty] private bool _paneOpen;
    [ObservableProperty] private SourceEditLinkViewModel? _sourceEditLinkViewModel;
    [ObservableProperty] private ViewModelBase? _currentPaneViewModel;

    private AdventureEditViewModel() { }

    public static async Task<AdventureEditViewModel> CreateAsync(int adventureId)
    {
        var appState = ApplicationState.Load();
        var instance = new AdventureEditViewModel();
        instance.Adventure = await TbspRpgDataServiceFactory.Load().AdventuresService.GetAdventureById(adventureId);
        instance.SourceEditLinkViewModel =
            await SourceEditLinkViewModel.CreateAsync(
                instance.Adventure.DescriptionSourceKey, instance.Adventure.Id, appState.Language);
        instance.PaneOpen = true;
        return instance;
    }
}