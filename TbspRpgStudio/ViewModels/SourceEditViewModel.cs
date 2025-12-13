using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class SourceEditViewModel : ViewModelBase
{
    [ObservableProperty] private Source? _source;

    public SourceEditViewModel(Source? source)
    {
        Source = source;
    }
}