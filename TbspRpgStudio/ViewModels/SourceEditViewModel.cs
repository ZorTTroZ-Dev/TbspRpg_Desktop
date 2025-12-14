using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.ViewModels;

public partial class SourceEditViewModel : ViewModelBase
{
    [ObservableProperty] private Source? _source;
    [ObservableProperty] private SourceEditModifyViewModel _sourceEditModifyViewModel;
    [ObservableProperty] private SourceEditSelectViewModel _sourceEditSelectViewModel;
    [ObservableProperty] private SourceEditCreateViewModel _sourceEditCreateViewModel;

    public SourceEditViewModel(Source? source)
    {
        Source = source;
        SourceEditModifyViewModel = new SourceEditModifyViewModel(Source);
        SourceEditSelectViewModel = new SourceEditSelectViewModel();
        SourceEditCreateViewModel = new SourceEditCreateViewModel();
    }
}