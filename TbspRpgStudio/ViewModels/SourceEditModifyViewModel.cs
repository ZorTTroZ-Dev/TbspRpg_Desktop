using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using TbspRpgDataLayer.Entities;

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
}