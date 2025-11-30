using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Services;
using TbspRpgProcessor.Entities;
using TbspRpgSettings;

namespace TbspRpgProcessor.Processors;

public interface IAdventureObjectProcessor
{
    Task RemoveAdventureObject(AdventureObjectRemoveModel adventureObjectRemoveModel);
    Task UpdateAdventureObject(AdventureObjectUpdateModel adventureObjectUpdateModel);
}

public class AdventureObjectProcessor: IAdventureObjectProcessor
{
    private readonly ISourceProcessor _sourceProcessor;
    private readonly IAdventureObjectService _adventureObjectService;
    private readonly ILocationsService _locationsService;
    private readonly ILogger _logger;

    public AdventureObjectProcessor(
        ISourceProcessor sourceProcessor,
        IAdventureObjectService adventureObjectService,
        ILocationsService locationsService,
        ILogger logger)
    {
        _sourceProcessor = sourceProcessor;
        _adventureObjectService = adventureObjectService;
        _locationsService = locationsService;
        _logger = logger;
    }

    public async Task RemoveAdventureObject(AdventureObjectRemoveModel adventureObjectRemoveModel)
    {
        var dbAdventureObject =
            await _adventureObjectService.GetAdventureObjectById(adventureObjectRemoveModel.AdventureObjectId);
        if (dbAdventureObject == null)
        {
            throw new ArgumentException("invalid adventure object id");
        }
        
        // eventually we'll have to deal with adventure object game state
        
        _adventureObjectService.RemoveAdventureObject(dbAdventureObject);
        await _adventureObjectService.SaveChanges();
    }

    public async Task UpdateAdventureObject(AdventureObjectUpdateModel adventureObjectUpdateModel)
    {
        AdventureObject dbAdventureObject = null;
        if (adventureObjectUpdateModel.AdventureObject.Id == TbspRpgUtilities.DB_EMPTY_ID)
        {
            dbAdventureObject = new AdventureObject()
            {
                Name = adventureObjectUpdateModel.AdventureObject.Name,
                Description = adventureObjectUpdateModel.AdventureObject.Description,
                Type = adventureObjectUpdateModel.AdventureObject.Type,
                AdventureId = adventureObjectUpdateModel.AdventureObject.AdventureId,
                Locations = new List<Location>()
            };
            foreach (var location in adventureObjectUpdateModel.AdventureObject.Locations)
            {
                _locationsService.AttachLocation(location);
                dbAdventureObject.Locations.Add(location);
            }
            await _adventureObjectService.AddAdventureObject(dbAdventureObject);
        }
        else
        {
            dbAdventureObject =
                await _adventureObjectService.GetAdventureObjectById(adventureObjectUpdateModel.AdventureObject.Id);
            if (dbAdventureObject == null)
            {
                throw new ArgumentException("invalid adventure object id");
            }

            dbAdventureObject.Name = adventureObjectUpdateModel.AdventureObject.Name;
            dbAdventureObject.Description = adventureObjectUpdateModel.AdventureObject.Description;
            dbAdventureObject.Type = adventureObjectUpdateModel.AdventureObject.Type;
            
            // reconcile attached locations
            if (dbAdventureObject.Locations == null)
                dbAdventureObject.Locations = new List<Location>();
            var locationsToRemove =
                dbAdventureObject.Locations.Except(adventureObjectUpdateModel.AdventureObject.Locations);
            var locationsToAdd =
                adventureObjectUpdateModel.AdventureObject.Locations.Except(dbAdventureObject.Locations);
            foreach (var location in locationsToRemove)
            {
                dbAdventureObject.Locations.Remove(location);
            }
            foreach (var location in locationsToAdd)
            {
                dbAdventureObject.Locations.Add(location);
            }
        }
        
        // update the source
        adventureObjectUpdateModel.NameSource.AdventureId = adventureObjectUpdateModel.AdventureObject.AdventureId;
        adventureObjectUpdateModel.DescriptionSource.AdventureId = adventureObjectUpdateModel.AdventureObject.AdventureId;
        if (string.IsNullOrEmpty(adventureObjectUpdateModel.NameSource.Name))
            adventureObjectUpdateModel.NameSource.Name = adventureObjectUpdateModel.AdventureObject.Name + "_name";
        if (string.IsNullOrEmpty(adventureObjectUpdateModel.DescriptionSource.Name))
            adventureObjectUpdateModel.DescriptionSource.Name = adventureObjectUpdateModel.AdventureObject.Name + "_desc";
        var dbNameSource = await _sourceProcessor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
            Source = adventureObjectUpdateModel.NameSource,
            Language = adventureObjectUpdateModel.Language
        });
        var dbDescSource = await _sourceProcessor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
            Source = adventureObjectUpdateModel.DescriptionSource,
            Language = adventureObjectUpdateModel.Language
        });
        dbAdventureObject.NameSourceKey = dbNameSource.Key;
        dbAdventureObject.DescriptionSourceKey = dbDescSource.Key;

        await _adventureObjectService.SaveChanges();
    }
}