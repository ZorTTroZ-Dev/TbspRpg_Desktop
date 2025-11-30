using System;
using TbspRpgDataLayer.Entities;

namespace TbspRpgStudio.Models;

public class AdventureView
{
    public string Name { get; set; }
    public string Description { get; set; }

    public static AdventureView FromAdventure(Adventure adventure)
    {
        return new AdventureView()
        {
            Name =  adventure.Name,
            Description = "Adventure View Description!"
        };
    }

    public static Adventure ToAdventure(AdventureView adventureView)
    {
        return new Adventure()
        {
            Name = adventureView.Name,
            DescriptionSourceKey = Guid.Empty,
            InitialSourceKey =  Guid.Empty,
            InitializationScriptId = null,
            TerminationScriptId = null
        };
    }
}