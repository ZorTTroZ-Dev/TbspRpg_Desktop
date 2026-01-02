using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureEditViewModel : ViewModelBase
{
    private const string CopyDestinationAdventureDescription = "COPY_DESTINATION_ADVENTURE_DESCRIPTION";
    private const string CopyDestinationAdventureInitial = "COPY_DESTINATION_ADVENTURE_INITIAL";

    [ObservableProperty] private Adventure? _adventure;
    [ObservableProperty] private bool _paneOpen;
    [ObservableProperty] private CopyEditLinkViewModel? _adventureDescriptionCopyEditLinkViewModel;
    [ObservableProperty] private CopyEditLinkViewModel? _adventureInitialCopyEditLinkViewModel;
    [ObservableProperty] private ViewModelBase? _currentPaneViewModel;

    private AdventureEditViewModel()
    {
        WeakReferenceMessenger.Default.Register<AdventureEditViewModel, CopyEditMessage>(this, (w, m) =>
        {
            CurrentPaneViewModel = m.SourceEditViewModel;
        });
        
        WeakReferenceMessenger.Default.Register<AdventureEditViewModel, AdventureEditCancelPaneMessage>(
            this, (w, m) =>
        {
            CurrentPaneViewModel = null;
        });
        
        WeakReferenceMessenger.Default.Register<AdventureEditViewModel, AdventureEditCopyChangedMessage>(this,
            async (w, m) =>
        {
            if (m.Destination == CopyDestinationAdventureDescription)
            {
                if (Adventure == null) return;
                if (Adventure.DescriptionCopyKey != m.Copy.Key)
                {
                    Adventure.DescriptionCopyKey = m.Copy.Key;
                    await TbspRpgDataServiceFactory.Load().AdventuresService.SaveChanges();
                }
                AdventureDescriptionCopyEditLinkViewModel =
                    await CopyEditLinkViewModel.CreateAsync(Adventure.DescriptionCopyKey,
                        CopyDestinationAdventureDescription);
                CurrentPaneViewModel = null;
            }
            
            if (m.Destination == CopyDestinationAdventureInitial)
            {
                if (Adventure == null) return;
                if (Adventure.InitialCopyKey != m.Copy.Key)
                {
                    Adventure.InitialCopyKey = m.Copy.Key;
                    await TbspRpgDataServiceFactory.Load().AdventuresService.SaveChanges();
                }
                AdventureInitialCopyEditLinkViewModel =
                    await CopyEditLinkViewModel.CreateAsync(Adventure.InitialCopyKey,
                        CopyDestinationAdventureInitial);
                CurrentPaneViewModel = null;
            }
        });
    }

    public static async Task<AdventureEditViewModel> CreateAsync(int adventureId)
    {
        var instance = new AdventureEditViewModel();
        instance.Adventure = await TbspRpgDataServiceFactory.Load().AdventuresService.GetAdventureById(adventureId);
        instance.AdventureDescriptionCopyEditLinkViewModel =
            await CopyEditLinkViewModel.CreateAsync(instance.Adventure.DescriptionCopyKey,
                CopyDestinationAdventureDescription);
        instance.AdventureInitialCopyEditLinkViewModel =
            await CopyEditLinkViewModel.CreateAsync(instance.Adventure.InitialCopyKey,
                CopyDestinationAdventureInitial);
        instance.PaneOpen = true;
        return instance;
    }
}