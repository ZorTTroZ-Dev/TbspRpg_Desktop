using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Services;
using TbspRpgProcessor.Entities;

namespace TbspRpgProcessor.Processors
{
    public interface IMapProcessor
    {
        Task ChangeLocationViaRoute(MapChangeLocationModel mapChangeLocationModel);
    }
    
    public class MapProcessor: IMapProcessor
    {
        private readonly IScriptProcessor _scriptProcessor;
        private readonly ISourceProcessor _sourceProcessor;
        private readonly IGamesService _gamesService;
        private readonly IRoutesService _routesService;
        private readonly IContentsService _contentsService;
        private readonly ILogger _logger;

        public MapProcessor(
            IScriptProcessor scriptProcessor,
            ISourceProcessor sourceProcessor,
            IGamesService gamesService,
            IRoutesService routesService,
            IContentsService contentsService,
            ILogger logger)
        {
            _scriptProcessor = scriptProcessor;
            _sourceProcessor = sourceProcessor;
            _gamesService = gamesService;
            _routesService = routesService;
            _contentsService = contentsService;
            _logger = logger;
        }
        
        // make sure the game exists, throw exception
        // make sure the route exists, throw exception
        // make sure the game is in the correct location, throw exception
        // check if the player can take route
        //  if fail add failure content to game
        //  if pass update location id, location time stamp, add pass content to game
        public async Task ChangeLocationViaRoute(MapChangeLocationModel mapChangeLocationModel)
        {
            var game = await _gamesService.GetGameByIdIncludeAdventure(mapChangeLocationModel.GameId);
            if (game == null)
            {
                throw new ArgumentException("invalid game id");
            }

            var route = await _routesService.GetRouteById(mapChangeLocationModel.RouteId);
            if (route == null)
            {
                throw new ArgumentException("invalid route id");
            }

            if (game.LocationId != route.LocationId)
            {
                throw new Exception("game not in location it should be");
            }
            
            // these scripts should just update game state, can't stop entering location
            // run the location exit script
            if (route.Location.ExitScriptId != null)
            {
                await _scriptProcessor.ExecuteScript(new ScriptExecuteModel()
                {
                    ScriptId = route.Location.ExitScriptId.GetValueOrDefault(),
                    Game = game
                });
            }
                
            // run the route taken script
            if (route.RouteTakenScriptId != null)
            {
                await _scriptProcessor.ExecuteScript(new ScriptExecuteModel()
                {
                    ScriptId = route.RouteTakenScriptId.GetValueOrDefault(),
                    Game = game
                });
            }
            
            // run the location enter script
            if (route.DestinationLocation.EnterScriptId != null)
            {
                await _scriptProcessor.ExecuteScript(new ScriptExecuteModel()
                {
                    ScriptId = route.DestinationLocation.EnterScriptId.GetValueOrDefault(),
                    Game = game
                });
            }

            // if we're entering the final location run the adventure completion script
            if (route.DestinationLocation.Final && game.Adventure.TerminationScriptId != null)
            {
                await _scriptProcessor.ExecuteScript(new ScriptExecuteModel()
                {
                    ScriptId = game.Adventure.TerminationScriptId.GetValueOrDefault(),
                    Game = game
                });
            }

            // for now assume the check passed
            var secondsSinceEpoch = new DateTimeOffset(mapChangeLocationModel.TimeStamp).ToUnixTimeMilliseconds();
            game.LocationId = route.DestinationLocationId;
            game.LocationUpdateTimeStamp = secondsSinceEpoch;

            // create content entry for the route
            await _contentsService.AddContent(new Content()
            {
                GameId = game.Id,
                Position = (ulong)secondsSinceEpoch,
                SourceKey = route.RouteTakenSourceKey
            });
            
            // create content entry for the new location
            await _contentsService.AddContent(new Content()
            {
                GameId = game.Id,
                Position = (ulong)secondsSinceEpoch + 1,
                SourceKey = route.DestinationLocation.SourceKey
            });

            // save context changes
            await _gamesService.SaveChanges();
        }
    }
}