using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories
{
    public interface ILocationsRepository : IBaseRepository
    {
        Task<Location> GetInitialForAdventure(int adventureId);
        Task<List<Location>> GetLocationsForAdventure(int adventureId);
        Task<Location> GetLocationById(int locationId);
        Task<Location> GetLocationByIdWithRoutes(int locationId);
        Task AddLocation(Location location);
        void RemoveLocation(Location location);
        void RemoveLocations(ICollection<Location> locations);
        Task<List<Location>> GetLocationsWithScript(int scriptId);
        Task<List<Location>> GetAdventureLocationsWithSource(int adventureId, Guid sourceKey);
        void AttachLocation(Location location);
        Task<Location> GetLocationByIdWithObjects(int locationId);
    }
    
    public class LocationsRepository: ILocationsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public LocationsRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Task<Location> GetInitialForAdventure(int adventureId)
        {
            return _databaseContext.Locations.AsQueryable()
                .Where(location => location.AdventureId == adventureId && location.Initial)
                .FirstOrDefaultAsync();
        }

        public Task<List<Location>> GetLocationsForAdventure(int adventureId)
        {
            return _databaseContext.Locations.AsQueryable()
                .Where(location => location.AdventureId == adventureId)
                .ToListAsync();
        }

        public Task<Location> GetLocationById(int locationId)
        {
            return _databaseContext.Locations.AsQueryable()
                .Include(location => location.Adventure)
                .FirstOrDefaultAsync(location => location.Id == locationId);
        }

        public Task<Location> GetLocationByIdWithRoutes(int locationId)
        {
            return _databaseContext.Locations.AsQueryable()
                .Include(location => location.Adventure)
                .Include(location => location.Routes)
                .FirstOrDefaultAsync(location => location.Id == locationId);
        }

        public async Task AddLocation(Location location)
        {
            await _databaseContext.AddAsync(location);
        }

        public void RemoveLocation(Location location)
        {
            _databaseContext.Remove(location);
        }

        public void RemoveLocations(ICollection<Location> locations)
        {
            _databaseContext.RemoveRange(locations);
        }

        public Task<List<Location>> GetLocationsWithScript(int scriptId)
        {
            return _databaseContext.Locations.AsQueryable()
                .Where(location => location.EnterScriptId == scriptId || location.ExitScriptId == scriptId)
                .ToListAsync();
        }

        public Task<List<Location>> GetAdventureLocationsWithSource(int adventureId, Guid sourceKey)
        {
            return _databaseContext.Locations.AsQueryable()
                .Where(location => location.AdventureId == adventureId && location.SourceKey == sourceKey)
                .ToListAsync();
        }

        public void AttachLocation(Location location)
        {
            _databaseContext.Attach(location);
        }

        public Task<Location> GetLocationByIdWithObjects(int locationId)
        {
            return _databaseContext.Locations.AsQueryable()
                .Include(location => location.AdventureObjects)
                .FirstOrDefaultAsync(location => location.Id == locationId);
        }

        public async Task SaveChanges()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}