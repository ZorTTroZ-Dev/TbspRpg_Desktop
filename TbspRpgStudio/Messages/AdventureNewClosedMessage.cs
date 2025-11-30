using TbspRpgStudio.Models;

namespace TbspRpgStudio.Messages;

public class AdventureNewClosedMessage(AdventureView? adventureView)
{
    public AdventureView? AdventureView { get; } = adventureView;
}