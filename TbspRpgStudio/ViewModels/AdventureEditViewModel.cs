using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureEditViewModel : ViewModelBase
{
    // ReSharper disable once InconsistentNaming
    public static readonly string COPY_DESTINATION_ADVENTURE_DESCRIPTION = "COPY_DESTINATION_ADVENTURE_DESCRIPTION";
    
    [ObservableProperty] private Adventure? _adventure;
    [ObservableProperty] private bool _paneOpen;
    [ObservableProperty] private CopyEditLinkViewModel? _copyEditLinkViewModel;
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
            if (m.Destination == COPY_DESTINATION_ADVENTURE_DESCRIPTION)
            {
                if (Adventure == null) return;
                if (Adventure.DescriptionSourceKey != m.Copy.Key)
                {
                    Adventure.DescriptionSourceKey = m.Copy.Key;
                    await TbspRpgDataServiceFactory.Load().AdventuresService.SaveChanges();
                }
                CopyEditLinkViewModel = await CopyEditLinkViewModel.CreateAsync(Adventure.DescriptionSourceKey);
                CurrentPaneViewModel = null;
            }
        });
    }

    public static async Task<AdventureEditViewModel> CreateAsync(int adventureId)
    {
        var instance = new AdventureEditViewModel();
        instance.Adventure = await TbspRpgDataServiceFactory.Load().AdventuresService.GetAdventureById(adventureId);
        instance.CopyEditLinkViewModel = await CopyEditLinkViewModel.CreateAsync(instance.Adventure.DescriptionSourceKey);
        instance.PaneOpen = true;
        return instance;
    }
}