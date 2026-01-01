using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.Messages;

public class AdventureEditCopyChangedMessage(Copy copy, string destination)
{
    public Copy Copy { get; } = copy;
    public string Destination { get; } = destination;
}