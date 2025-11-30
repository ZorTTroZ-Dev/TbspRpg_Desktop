using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Services;
using TbspRpgProcessor.Entities;
using TbspRpgSettings;

namespace TbspRpgProcessor.Processors
{
    public interface ILocationProcessor
    {
        Task UpdateLocation(LocationUpdateModel locationUpdateModel);
        Task RemoveLocation(LocationRemoveModel locationRemoveModel);
        Task RemoveLocations(LocationsRemoveModel locationsRemoveModel);
    }
    
    public class LocationProcessor : ILocationProcessor
    {
        private readonly ISourceProcessor _sourceProcessor;
        private readonly ILocationsService _locationsService;
        private readonly IRoutesService _routesService;
        private readonly IAdventureObjectService _adventureObjectService;
        private readonly ILogger _logger;

        public LocationProcessor(
            ISourceProcessor sourceProcessor,
            ILocationsService locationsService,
            IRoutesService routesService,
            IAdventureObjectService adventureObjectService,
            ILogger logger)
        {
            _sourceProcessor = sourceProcessor;
            _locationsService = locationsService;
            _routesService = routesService;
            _adventureObjectService = adventureObjectService;
            _logger = logger;
        }
        
        public async Task UpdateLocation(LocationUpdateModel locationUpdateModel)
        {
            Location dbLocation = null;
            if (locationUpdateModel.Location.Id == TbspRpgUtilities.DB_EMPTY_ID)
            {
                dbLocation = new Location()
                {
                    Name = locationUpdateModel.Location.Name,
                    Initial = locationUpdateModel.Location.Initial,
                    Final = locationUpdateModel.Location.Final,
                    AdventureId = locationUpdateModel.Location.AdventureId,
                    EnterScriptId = locationUpdateModel.Location.EnterScriptId,
                    ExitScriptId = locationUpdateModel.Location.ExitScriptId,
                    AdventureObjects = new List<AdventureObject>()
                };
                foreach (var adventureObject in locationUpdateModel.Location.AdventureObjects)
                {
                    _adventureObjectService.AttachObject(adventureObject);
                    dbLocation.AdventureObjects.Add(adventureObject);
                }
                await _locationsService.AddLocation(dbLocation);
            }
            else
            {
                // load the location, update the name and initial fields, save it
                // need to get the location with adventure objects
                dbLocation = await _locationsService.GetLocationByIdWithObjects(locationUpdateModel.Location.Id);
                if (dbLocation == null)
                    throw new ArgumentException("invalid location id");
                dbLocation.Name = locationUpdateModel.Location.Name;
                dbLocation.Initial = locationUpdateModel.Location.Initial;
                dbLocation.Final = locationUpdateModel.Location.Final;
                dbLocation.EnterScriptId = locationUpdateModel.Location.EnterScriptId;
                dbLocation.ExitScriptId = locationUpdateModel.Location.ExitScriptId;
                
                // reconcile attached adventure objects
                if (dbLocation.AdventureObjects == null)
                    dbLocation.AdventureObjects = new List<AdventureObject>();
                var adventureObjectsToRemove =
                    dbLocation.AdventureObjects.Except(locationUpdateModel.Location.AdventureObjects);
                var adventureObjectsToAdd =
                    locationUpdateModel.Location.AdventureObjects.Except(dbLocation.AdventureObjects);
                foreach (var adventureObject in adventureObjectsToRemove)
                {
                    dbLocation.AdventureObjects.Remove(adventureObject);
                }
                foreach (var adventureObject in adventureObjectsToAdd)
                {
                    dbLocation.AdventureObjects.Add(adventureObject);
                }
            }
            
            // update the source
            locationUpdateModel.Source.AdventureId = locationUpdateModel.Location.AdventureId;
            if (string.IsNullOrEmpty(locationUpdateModel.Source.Name))
                locationUpdateModel.Source.Name = locationUpdateModel.Location.Name;
            var dbSource = await _sourceProcessor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
                Source = locationUpdateModel.Source,
                Language = locationUpdateModel.Language
            });
            dbLocation.SourceKey = dbSource.Key;
            
            // save the changes
            await _locationsService.SaveChanges();
        }

        public async Task RemoveLocation(LocationRemoveModel locationRemoveModel)
        {
            var dbLocation = await _locationsService.GetLocationByIdWithRoutes(locationRemoveModel.LocationId);
            if (dbLocation == null)
            {
                throw new ArgumentException("invalid location id");
            }

            await RemoveLocation(dbLocation);
        }

        public async Task RemoveLocations(LocationsRemoveModel locationsRemoveModel)
        {
            foreach (var location in locationsRemoveModel.Locations)
            {
                await RemoveLocation(location, false);
            }

            if (locationsRemoveModel.Save)
                await _locationsService.SaveChanges();
        }
        
        private async Task RemoveLocation(Location location, bool save = true)
        {
            if (location.Routes != null && location.Routes.Count > 0)
            {
                _routesService.RemoveRoutes(location.Routes);
            }
            _locationsService.RemoveLocation(location);
            
            if(save)
                await _locationsService.SaveChanges();
        }
    }
}