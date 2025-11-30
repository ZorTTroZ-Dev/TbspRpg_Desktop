using CommunityToolkit.Mvvm.ComponentModel;

namespace TbspRpgStudio.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPageViewModel;

    public MainWindowViewModel()
    {
        _currentPageViewModel = new AdventureListViewModel();
    }
}