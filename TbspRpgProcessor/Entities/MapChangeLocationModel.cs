using System;

namespace TbspRpgProcessor.Entities;

public class MapChangeLocationModel
{
    public int GameId { get; set; }
    public int RouteId { get; set; }
    public DateTime TimeStamp { get; set; }
}