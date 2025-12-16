using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.Messages;

public class SourceEditAdventureDescriptionMessage(Source source)
{
    public Source Source { get; } = source;
}