using System;
using System.Collections.Generic;

namespace TbspRpgDataLayer.Entities;

public class Script
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
    public int AdventureId { get; set; }
    
    public ICollection<Script> IncludedIn { get; set; }
    public ICollection<Script> Includes { get; set; }
    public Adventure Adventure { get; set; }
    public ICollection<Adventure> AdventureTerminations { get; set; }
    public ICollection<Adventure> AdventureInitializations { get; set; }
}