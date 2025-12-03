using System;
using System.Threading.Tasks;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor;

namespace TbspRpgStudio.Models;

public class AdventureView
{
    public string Name { get; set; }
    public string Description { get; set; }

    public static async Task<AdventureView> FromAdventure(Adventure adventure)
    {
        // load the description
        var sourceService = TbspRpgDataServiceFactory.Load().SourcesService;
        var description = await sourceService.GetSourceTextForKey(adventure.DescriptionSourceKey);
        return new AdventureView()
        {
            Name =  adventure.Name,
            Description = description
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