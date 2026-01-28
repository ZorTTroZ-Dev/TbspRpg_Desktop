using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class CopyEditLinkViewModel : ViewModelBase
{ 
    private Copy? _copy;
    [ObservableProperty] private string _copyText = "";
    [ObservableProperty] private string _copyKey = "";
    private readonly string _copyDestination;

    private CopyEditLinkViewModel(string copyDestination)
    {
        _copyDestination = copyDestination;
    }

    public static async Task<CopyEditLinkViewModel?> CreateAsync(Guid copyKey, string  copyDestination)
    {
        var instance = new CopyEditLinkViewModel(copyDestination);
        var appState = ApplicationState.Load();
        instance._copy = await TbspRpgDataServiceFactory.Load().CopyService.GetCopy(copyKey, appState.Language);
        instance.CopyKey = instance._copy == null ? "" : instance._copy.Key.ToString();
        instance.CopyText = instance._copy == null ? "" : instance._copy.Text;
        return instance;
    }

    [RelayCommand]
    public void EditCopy()
    {
        if (_copy != null)
            WeakReferenceMessenger.Default.Send(new CopyEditMessage(new CopyEditViewModel(_copy.Key, _copyDestination)));
    }
}