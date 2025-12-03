using AdventureViewModel = TbspRpgStudio.ViewModels.AdventureViewModel;

namespace TbspRpgStudio.Messages;

public class AdventureNewClosedMessage(AdventureViewModel? adventureViewModel)
{
    public AdventureViewModel? AdventureViewModel { get; } = adventureViewModel;
}