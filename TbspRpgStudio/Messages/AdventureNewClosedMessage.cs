using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.Messages;

public class AdventureNewClosedMessage(Adventure? adventure)
{
    public Adventure Adventure { get; set; } = adventure;
}