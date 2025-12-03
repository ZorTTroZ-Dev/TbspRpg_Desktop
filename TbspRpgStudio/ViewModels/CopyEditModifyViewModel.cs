using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor;
using TbspRpgProcessor.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class CopyEditModifyViewModel : ViewModelBase
{
    [ObservableProperty] Copy _copy;

    public CopyEditModifyViewModel(Copy copy)
    {
        Copy = copy;
    }

    [RelayCommand]
    public void CancelEdit()
    {
        WeakReferenceMessenger.Default.Send(new AdventureEditCancelPaneMessage());
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        Copy = await TbspRpgProcessorFactory.TbspRpgProcessor().UpdateCopy(new CopyUpdateModel()
        {
            Save = true,
            Copy = Copy
        });
        WeakReferenceMessenger.Default.Send(new AdventureEditCopyChangedMessage(Copy,
            AdventureEditViewModel.COPY_DESTINATION_ADVENTURE_DESCRIPTION));
    }
}