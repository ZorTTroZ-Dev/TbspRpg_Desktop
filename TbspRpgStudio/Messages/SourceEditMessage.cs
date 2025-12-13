using TbspRpgStudio.ViewModels;

namespace TbspRpgStudio.Messages;

public class SourceEditMessage(SourceEditViewModel? sourceEditViewModel)
{
    public SourceEditViewModel? SourceEditViewModel { get; } = sourceEditViewModel;
}