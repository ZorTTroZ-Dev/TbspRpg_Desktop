using System;

namespace TbspRpgProcessor.Entities;

public class ContentTextForKeyModel
{
    public int GameId { get; set; }
    public Guid SourceKey { get; set; }
    public bool Processed { get; set; } = false;
}