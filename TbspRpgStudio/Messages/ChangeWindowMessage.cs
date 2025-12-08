using CommunityToolkit.Mvvm.Messaging.Messages;
using TbspRpgStudio.ViewModels;

namespace TbspRpgStudio.Messages;

public class ChangeWindowMessage (ViewModelBase viewModel)
{
    public ViewModelBase ViewModel { get; } = viewModel;
}