using System;

namespace TbspRpgDataLayer.Entities;

public class AdventureObjectGameState
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int AdventureObjectId { get; set; }
    public string AdventureObjectState { get; set; }
    
    public Game Game { get; set; }
    public AdventureObject AdventureObject { get; set; }
}