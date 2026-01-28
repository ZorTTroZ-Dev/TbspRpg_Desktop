using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.Messages;

public class ScriptEditNewPrimary(Script script)
{
    public Script Script { get; } = script;
}