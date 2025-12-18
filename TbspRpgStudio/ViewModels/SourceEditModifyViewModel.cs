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

public partial class SourceEditModifyViewModel : ViewModelBase
{
    [ObservableProperty] List<string> _languages = TbspRpgSettings.Settings.Languages.GetAllLanguages();
    [ObservableProperty] string? _selectedLanguage = TbspRpgSettings.Settings.Languages.DEFAULT;
    [ObservableProperty] Source? _source;

    public SourceEditModifyViewModel(Source? source)
    {
        Source = source;
    }

    [RelayCommand]
    public void CancelEdit()
    {
        WeakReferenceMessenger.Default.Send(new AdventureEditCancelPaneMessage());
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        Source = await TbspRpgProcessorFactory.TbspRpgProcessor().CreateOrUpdateSource(new SourceCreateOrUpdateModel()
        {
            Language = SelectedLanguage,
            Save = true,
            Source = Source
        });
        WeakReferenceMessenger.Default.Send(new AdventureEditSourceChangedMessage(Source,
            AdventureEditViewModel.SOURCE_DESTINATION_ADVENTURE_DESCRIPTION));
    }
}