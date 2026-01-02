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
    private readonly string _copyDestination;

    public CopyEditViewModel(Guid copyKey, string copyDestination)
    {
        _copyKey = copyKey;
        _copyDestination = copyDestination;
        var languagesService = TbspRpgDataServiceFactory.Load().LanguagesService;
        Languages = languagesService.GetAllLanguages();
        SelectedLanguage = languagesService.GetDefaultLanguage();
        Task.Run(UpdateViews).Wait();
    }

    private async Task UpdateViews()
    {
        var copyService = TbspRpgDataServiceFactory.Load().CopyService;
        var modifyCopy = await copyService.GetCopy(_copyKey, SelectedLanguage);
        CopyEditModifyViewModel = new CopyEditModifyViewModel(modifyCopy, _copyDestination);
        var selectCopy = await copyService.GetCopyForLanguage(SelectedLanguage);
        CopyEditSelectViewModel = new CopyEditSelectViewModel(selectCopy, _copyDestination);
        CopyEditCreateViewModel = new CopyEditCreateViewModel(modifyCopy, Languages,  _copyDestination);
    }
    
    partial void OnSelectedLanguageChanged(Language? value)
    {
        if (value != null)
            Task.Run(UpdateViews).Wait();
    }
}