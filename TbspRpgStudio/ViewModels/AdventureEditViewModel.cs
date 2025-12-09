using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureEditViewModel : ViewModelBase
{
    [ObservableProperty] private Adventure? _adventure;
    [ObservableProperty] private bool _paneOpen;
    [ObservableProperty] private SourceEditViewModel? _sourceEditViewModel;

    private AdventureEditViewModel() { }

    public static async Task<AdventureEditViewModel> CreateAsync(int adventureId)
    {
        var instance = new AdventureEditViewModel();
        instance.Adventure = await TbspRpgDataServiceFactory.Load().AdventuresService.GetAdventureById(adventureId);
        instance.SourceEditViewModel = new SourceEditViewModel();
        instance.PaneOpen = true;
        return instance;
    }
}