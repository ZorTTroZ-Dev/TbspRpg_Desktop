using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class SourceEditModifyViewModel : ViewModelBase
{
    [ObservableProperty] List<string> _languages = TbspRpgSettings.Settings.Languages.GetAllLanguages();
    [ObservableProperty] string? _selectedLanguage;
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
}