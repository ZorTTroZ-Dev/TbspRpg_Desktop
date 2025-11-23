using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services
{
    public interface ILocationsService : IBaseService
    {
        Task<Location> GetInitialLocationForAdventure(int adventureId);
        Task<List<Location>> GetLocationsForAdventure(int adventureId);
        Task<Location> GetLocationById(int locationId);
        Task<Location> GetLocationByIdWithRoutes(int locationId);
        Task AddLocation(Location location);
        void RemoveLocation(Location location);
        void RemoveLocations(ICollection<Location> locations);
        Task RemoveScriptFromLocations(int scriptId);
        Task<bool> DoesAdventureLocationUseSource(int adventureId, Guid sourceKey);
        Task<List<Location>> GetAdventureLocationsWithSource(int adventureId, Guid sourceKey);
        void AttachLocation(Location location);
        Task<Location> GetLocationByIdWithObjects(int locationId);
    }
    
    public class LocationsService : ILocationsService
    {
        private readonly ILocationsRepository _locationsRepository;
        private readonly ILogger<LocationsService> _logger;

        public LocationsService(
            ILocationsRepository locationsRepository,
            ILogger<LocationsService> logger)
        {
            _locationsRepository = locationsRepository;
            _logger = logger;
        }

        public Task<Location> GetInitialLocationForAdventure(int adventureId)
        {
            return _locationsRepository.GetInitialForAdventure(adventureId);
        }

        public Task<List<Location>> GetLocationsForAdventure(int adventureId)
        {
            return _locationsRepository.GetLocationsForAdventure(adventureId);
        }

        public Task<Location> GetLocationById(int locationId)
        {
            return _locationsRepository.GetLocationById(locationId);
        }

        public Task<Location> GetLocationByIdWithRoutes(int locationId)
        {
            return _locationsRepository.GetLocationByIdWithRoutes(locationId);
        }

        public async Task AddLocation(Location location)
        {
            await _locationsRepository.AddLocation(location);
        }

        public void RemoveLocation(Location location)
        {
            _locationsRepository.RemoveLocation(location);
        }

        public void RemoveLocations(ICollection<Location> locations)
        {
            _locationsRepository.RemoveLocations(locations);
        }

        public async Task RemoveScriptFromLocations(int scriptId)
        {
            var locations = await _locationsRepository.GetLocationsWithScript(scriptId);
            foreach (var location in locations)
            {
                if (location.EnterScriptId == scriptId)
                    location.EnterScriptId = null;
                if (location.ExitScriptId == scriptId)
                    location.ExitScriptId = null;
            }
        }

        public async Task<bool> DoesAdventureLocationUseSource(int adventureId, Guid sourceKey)
        {
            var locations = await GetAdventureLocationsWithSource(adventureId, sourceKey);
            return locations.Any();
        }

        public Task<List<Location>> GetAdventureLocationsWithSource(int adventureId, Guid sourceKey)
        {
            return _locationsRepository.GetAdventureLocationsWithSource(adventureId, sourceKey);
        }

        public void AttachLocation(Location location)
        {
            _locationsRepository.AttachLocation(location);
        }

        public Task<Location> GetLocationByIdWithObjects(int locationId)
        {
            return _locationsRepository.GetLocationByIdWithObjects(locationId);
        }

        public async Task SaveChanges()
        {
            await _locationsRepository.SaveChanges();
        }
    }
}