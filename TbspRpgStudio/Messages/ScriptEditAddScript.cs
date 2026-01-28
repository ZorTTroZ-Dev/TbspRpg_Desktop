using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.Messages;

public class ScriptEditAddScript(Script? script, string? name = null)
{
    public Script? Script { get; } = script;
    public string? Name { get; } = name;
}