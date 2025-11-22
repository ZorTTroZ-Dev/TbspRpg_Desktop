using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services
{
    public interface IRoutesService: IBaseService
    {
        Task<List<Route>> GetRoutesForLocation(int locationId);
        Task<List<Route>> GetRoutesForAdventure(int adventureId);
        Task<Route> GetRouteById(int routeId);
        Task<List<Route>> GetRoutes(RouteFilter routeFilter);
        void RemoveRoute(Route route);
        void RemoveRoutes(ICollection<Route> routes);
        Task AddRoute(Route route);
        Task RemoveScriptFromRoutes(int scriptId);
        Task<bool> DoesAdventureRouteUseSource(int adventureId, Guid sourceKey);
        Task<List<Route>> GetAdventureRoutesWithSource(int adventureId, Guid sourceKey);
    }
    
    public class RoutesService : IRoutesService
    {
        private readonly IRoutesRepository _routesRepository;
        private readonly ILogger<RoutesService> _logger;

        public RoutesService(IRoutesRepository routesRepository, ILogger<RoutesService> logger)
        {
            _routesRepository = routesRepository;
            _logger = logger;
        }

        public Task<List<Route>> GetRoutesForLocation(int locationId)
        {
            return _routesRepository.GetRoutesForLocation(locationId);
        }

        public Task<List<Route>> GetRoutesForAdventure(int adventureId)
        {
            return _routesRepository.GetRoutesForAdventure(adventureId);
        }

        public Task<Route> GetRouteById(int routeId)
        {
            return _routesRepository.GetRouteById(routeId);
        }

        public Task<List<Route>> GetRoutes(RouteFilter routeFilter)
        {
            return _routesRepository.GetRoutes(routeFilter);
        }

        public void RemoveRoute(Route route)
        {
            _routesRepository.RemoveRoute(route);
        }

        public void RemoveRoutes(ICollection<Route> routes)
        {
            _routesRepository.RemoveRoutes(routes);
        }

        public async Task AddRoute(Route route)
        {
            await _routesRepository.AddRoute(route);
        }

        public async Task RemoveScriptFromRoutes(int scriptId)
        {
            var routes = await _routesRepository.GetRoutesWithScript(scriptId);
            foreach (var route in routes)
            {
                if (route.RouteTakenScriptId == scriptId)
                    route.RouteTakenScriptId = null;
            }
        }

        public async Task<bool> DoesAdventureRouteUseSource(int adventureId, Guid sourceKey)
        {
            var routes = await GetAdventureRoutesWithSource(adventureId, sourceKey);
            return routes.Any();
        }

        public Task<List<Route>> GetAdventureRoutesWithSource(int adventureId, Guid sourceKey)
        {
            return _routesRepository.GetAdventureRoutesWithSource(adventureId, sourceKey);
        }

        public async Task SaveChanges()
        {
            await _routesRepository.SaveChanges();
        }
    }
}