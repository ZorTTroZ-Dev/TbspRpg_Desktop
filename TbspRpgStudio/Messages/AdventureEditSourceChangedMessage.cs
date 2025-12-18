using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.Messages;

public class AdventureEditSourceChangedMessage(Source source, string destination)
{
    public Source Source { get; } = source;
    public string Destination { get; } = destination;
}