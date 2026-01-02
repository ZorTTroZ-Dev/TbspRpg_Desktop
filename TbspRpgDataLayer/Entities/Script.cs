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
    
    public ICollection<ScriptInclude> IncludedIn { get; private set; }
    public ICollection<ScriptInclude> Includes { get; private set; }
    public Adventure Adventure { get; set; }
    public ICollection<Adventure> AdventureTerminations { get; set; }
    public ICollection<Adventure> AdventureInitializations { get; set; }
}

public class ScriptInclude
{
    public int IncludedInId { get; set; }
    public int IncludesId { get; set; }
    public int Order  { get; set; }
    
    public Script Includes { get; set; }
    public Script IncludedIn { get; set; }
}