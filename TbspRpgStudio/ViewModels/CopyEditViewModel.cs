using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.ViewModels;

public partial class CopyEditViewModel : ViewModelBase
{
    private readonly Guid _copyKey;
    [ObservableProperty] List<Language> _languages;
    [ObservableProperty] Language _selectedLanguage;
    [ObservableProperty] private CopyEditModifyViewModel _copyEditModifyViewModel = null!;
    [ObservableProperty] private CopyEditSelectViewModel _copyEditSelectViewModel = null!;
    [ObservableProperty] private CopyEditCreateViewModel _copyEditCreateViewModel = null!;
    private readonly bool _copyEnabled;
    [ObservableProperty] private bool _readOnly = false;
    [ObservableProperty] private int _selectedIndex;
    private readonly string _copyDestination;

    public CopyEditViewModel(Guid copyKey, string copyDestination, bool readOnly = false, bool copyEnabled = false)
    {
        _copyKey = copyKey;
        _copyDestination = copyDestination;
        var languagesService = TbspRpgDataServiceFactory.Load().LanguagesService;
        Languages = languagesService.GetAllLanguages();
        SelectedLanguage = languagesService.GetDefaultLanguage();
        _copyEnabled = copyEnabled;
        ReadOnly = readOnly;
        SelectedIndex = 0;
        if (ReadOnly)
            SelectedIndex = 1;
        Task.Run(UpdateViews).Wait();
    }

    private async Task UpdateViews()
    {
        var copyService = TbspRpgDataServiceFactory.Load().CopyService;
        var modifyCopy = await copyService.GetCopy(_copyKey, SelectedLanguage);
        CopyEditModifyViewModel = new CopyEditModifyViewModel(modifyCopy, _copyDestination);
        var selectCopy = await copyService.GetCopyForLanguage(SelectedLanguage);
        CopyEditSelectViewModel = new CopyEditSelectViewModel(selectCopy, _copyDestination, ReadOnly, _copyEnabled);
        CopyEditCreateViewModel = new CopyEditCreateViewModel(modifyCopy, Languages,  _copyDestination, ReadOnly, _copyEnabled);
    }
    
    partial void OnSelectedLanguageChanged(Language? value)
    {
        if (value != null)
            Task.Run(UpdateViews).Wait();
    }
}