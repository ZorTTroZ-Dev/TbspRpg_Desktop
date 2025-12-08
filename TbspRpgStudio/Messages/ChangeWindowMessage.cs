using CommunityToolkit.Mvvm.Messaging.Messages;
using TbspRpgStudio.ViewModels;

namespace TbspRpgStudio.Messages;

public class ChangeWindowMessage (string windowName)
{
    public string WindowName { get; } = windowName;
}