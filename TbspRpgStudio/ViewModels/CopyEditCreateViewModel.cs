using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor;
using TbspRpgProcessor.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class CopyEditCreateViewModel(Copy copy, List<Language> languages, string copyDestination) : ViewModelBase
{
    [ObservableProperty] private string _text;
    [ObservableProperty] private string _name;
    [ObservableProperty] private bool _replace;
    [ObservableProperty] private Copy _copy = copy;

    [RelayCommand]
    public void CancelEdit()
    {
        WeakReferenceMessenger.Default.Send(new AdventureEditCancelPaneMessage());
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (Replace)
        {
            Copy.Text = Text;
            Copy.Name = Name;
            Copy = await TbspRpgProcessorFactory.TbspRpgProcessor().UpdateCopy(new CopyUpdateModel()
            {
                Save = true,
                Copy = Copy
            });
        }
        else
        {
            var newCopy = new Copy()
            {
                Name = Name,
                Text = Text,
                AdventureId = Copy.AdventureId,
                Language = Copy.Language
            };
            Copy = await TbspRpgProcessorFactory.TbspRpgProcessor().CreateCopy(new CopyCreateModel()
            {
                Save = true,
                Copy = newCopy,
                Languages =  languages
            });
        }

        WeakReferenceMessenger.Default.Send(new AdventureEditCopyChangedMessage(Copy, copyDestination));
    }
}