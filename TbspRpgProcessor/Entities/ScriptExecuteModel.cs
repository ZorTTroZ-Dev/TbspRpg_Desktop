using System;
using TbspRpgDataLayer.Entities;

namespace TbspRpgProcessor.Entities;

public class ScriptExecuteModel
{
    public int ScriptId { get; set; }
    public int GameId { get; set; }
    public Script Script { get; set; }
    public Game Game { get; set; }
}