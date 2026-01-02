using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.Messages;

public class AdventureEditScriptChangedMessage(Script? script, string destination, bool closeEditor)
{
    public Script? Script { get; } = script;
    public string Destination { get; } = destination;
    public bool CloseEditor { get; } = closeEditor;
}