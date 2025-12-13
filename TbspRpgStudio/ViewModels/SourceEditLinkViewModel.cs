using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class SourceEditLinkViewModel : ViewModelBase
{
    [ObservableProperty] private Source? _source;

    private SourceEditLinkViewModel() { }

    public static async Task<SourceEditLinkViewModel?> CreateAsync(Guid sourceKey, int adventureId, string language)
    {
        var instance = new SourceEditLinkViewModel();
        instance.Source = await TbspRpgDataServiceFactory.Load().SourcesService.GetSourceForKey(sourceKey, adventureId, language);
        return instance;
    }

    [RelayCommand]
    public void EditSource()
    {
        WeakReferenceMessenger.Default.Send(new SourceEditMessage(new SourceEditViewModel(Source)));
    }
}