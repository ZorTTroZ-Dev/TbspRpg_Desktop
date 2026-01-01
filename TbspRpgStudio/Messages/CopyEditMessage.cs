using TbspRpgStudio.ViewModels;

namespace TbspRpgStudio.Messages;

public class CopyEditMessage(CopyEditViewModel? sourceEditViewModel)
{
    public CopyEditViewModel? SourceEditViewModel { get; } = sourceEditViewModel;
}