using TbspRpgStudio.ViewModels;

namespace TbspRpgStudio.Messages;

public class ScriptEditMessage(ScriptEditViewModel? scriptEditViewModel)
{
    public ScriptEditViewModel? ScriptEditViewModel { get; } = scriptEditViewModel;
}