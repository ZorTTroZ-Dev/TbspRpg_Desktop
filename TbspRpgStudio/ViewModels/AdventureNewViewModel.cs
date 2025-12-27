using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class AdventureNewViewModel : ViewModelBase
{
    [ObservableProperty] private string? _name;
    [ObservableProperty] private string? _description;
    [ObservableProperty] private List<LanguageViewModel>? _languages;

    public AdventureNewViewModel()
    {
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var languages = await TbspRpgDataServiceFactory.Load().LanguagesService.GetAllLanguagesAsync();
        Languages = languages.Select(lang => LanguageViewModel.FromLanguage(lang)).ToList();
    }

    [RelayCommand (CanExecute = nameof(CanSaveAdventure))]
    public void SaveAdventure()
    {
        var selectedLanguages = Languages.Where(lang => lang.IsSelected)
            .Select(lang => lang.Language).ToList();
        var newAdventureView = new AdventureViewModel()
        {
            Name = Name,
            Description = Description,
            Languages = selectedLanguages
        };
        WeakReferenceMessenger.Default.Send(new AdventureNewClosedMessage(newAdventureView));
    }

    private bool CanSaveAdventure()
    {
        return Name != null;
    }
    
    [RelayCommand]
    public void CancelAdventure()
    {
        WeakReferenceMessenger.Default.Send(new AdventureNewClosedMessage(null));
    }
}