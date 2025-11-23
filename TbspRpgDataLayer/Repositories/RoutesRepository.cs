using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories
{
    public interface IRoutesRepository : IBaseRepository
    {
        Task<List<Route>> GetRoutesForLocation(int locationId);
        Task<List<Route>> GetRoutesForAdventure(int adventureId);
        Task<Route> GetRouteById(int routeId);
        Task<List<Route>> GetRoutes(RouteFilter routeFilter);
        void RemoveRoute(Route route);
        void RemoveRoutes(ICollection<Route> routes);
        Task AddRoute(Route route);
        Task<List<Route>> GetRoutesWithScript(int scriptId);
        Task<List<Route>> GetAdventureRoutesWithSource(int adventureId, Guid sourceKey);
    }
    
    public class RoutesRepository : IRoutesRepository
    {
        private readonly DatabaseContext _databaseContext;

        public RoutesRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Task<List<Route>> GetRoutes(RouteFilter routeFilter)
        {
            var query = _databaseContext.Routes.AsQueryable();
            if (routeFilter != null)
            {
                if (routeFilter.DestinationLocationId != null)
                {
                    query = query.Where(route => route.DestinationLocationId == routeFilter.DestinationLocationId);
                }
                if (routeFilter.LocationId != null)
                {
                    query = query.Where(route => route.LocationId == routeFilter.LocationId);
                }
                if (routeFilter.AdventureId != null)
                {
                    query = query.Where(route => route.Location.AdventureId == routeFilter.AdventureId);
                }
            }
            return query.ToListAsync();
        }

        public void RemoveRoute(Route route)
        {
            _databaseContext.Remove(route);
        }

        public void RemoveRoutes(ICollection<Route> routes)
        {
            _databaseContext.RemoveRange(routes);
        }

        public async Task AddRoute(Route route)
        {
            await _databaseContext.AddAsync(route);
        }

        public Task<List<Route>> GetRoutesWithScript(int scriptId)
        {
            return _databaseContext.Routes.AsQueryable()
                .Where(route => route.RouteTakenScriptId == scriptId)
                .ToListAsync();
        }

        public Task<List<Route>> GetAdventureRoutesWithSource(int adventureId, Guid sourceKey)
        {
            return _databaseContext.Routes.AsQueryable()
                .Where(route => route.Location.AdventureId == adventureId &&
                                (route.SourceKey == sourceKey || route.RouteTakenSourceKey == sourceKey))
                .ToListAsync();
        }

        public Task<List<Route>> GetRoutesForLocation(int locationId)
        {
            return GetRoutes(new RouteFilter()
            {
                LocationId = locationId
            });
        }

        public Task<List<Route>> GetRoutesForAdventure(int adventureId)
        {
            return GetRoutes(new RouteFilter()
            {
                AdventureId = adventureId
            });
        }

        public Task<Route> GetRouteById(int routeId)
        {
            return _databaseContext.Routes.AsQueryable()
                .Include(route => route.DestinationLocation)
                .Include(route => route.Location)
                .FirstOrDefaultAsync(route => route.Id == routeId);
        }

        public async Task SaveChanges()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}