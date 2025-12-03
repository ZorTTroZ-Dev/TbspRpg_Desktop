using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.ViewModels;

public partial class CopyEditViewModel : ViewModelBase
{
    private Guid _copyKey;
    [ObservableProperty] List<Language> _languages;
    [ObservableProperty] Language _selectedLanguage;
    [ObservableProperty] private CopyEditModifyViewModel _copyEditModifyViewModel = null!;
    [ObservableProperty] private CopyEditSelectViewModel _copyEditSelectViewModel = null!;
    [ObservableProperty] private CopyEditCreateViewModel _copyEditCreateViewModel = null!;

    public CopyEditViewModel(Guid copyKey)
    {
        _copyKey = copyKey;
        var languagesService = TbspRpgDataServiceFactory.Load().LanguagesService;
        Languages = languagesService.GetAllLanguages();
        SelectedLanguage = languagesService.GetDefaultLanguage();
        Task.Run(UpdateViews).Wait();
    }

    private async Task UpdateViews()
    {
        var copyService = TbspRpgDataServiceFactory.Load().CopyService;
        var modifyCopy = await copyService.GetCopy(_copyKey, SelectedLanguage);
        CopyEditModifyViewModel = new CopyEditModifyViewModel(modifyCopy);
        var selectCopy = await copyService.GetCopyForLanguage(SelectedLanguage);
        CopyEditSelectViewModel = new CopyEditSelectViewModel(selectCopy);
        CopyEditCreateViewModel = new CopyEditCreateViewModel(modifyCopy, Languages);
    }
    
    partial void OnSelectedLanguageChanged(Language? value)
    {
        if (value != null)
            Task.Run(UpdateViews).Wait();
    }
}