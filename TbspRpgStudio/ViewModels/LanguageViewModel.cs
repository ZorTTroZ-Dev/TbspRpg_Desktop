using CommunityToolkit.Mvvm.ComponentModel;
using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.ViewModels;

public partial class LanguageViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private Language? _language;

    public static LanguageViewModel FromLanguage(Language language, bool isSelected = false)
    {
        return new LanguageViewModel()
        {
            Language = language,
            IsSelected = isSelected
        };
    }
}