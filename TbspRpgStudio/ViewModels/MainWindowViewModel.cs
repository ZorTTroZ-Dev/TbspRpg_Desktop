using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgStudio.Messages;
using TbspRpgStudio.Views;

namespace TbspRpgStudio.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPageViewModel;

    public MainWindowViewModel()
    {
        _currentPageViewModel = new AdventureListViewModel();
        
        WeakReferenceMessenger.Default.Register<MainWindowViewModel, ChangeWindowMessage>(this, (w, m) =>
        {
            CurrentPageViewModel = m.ViewModel;
        });
    }
}