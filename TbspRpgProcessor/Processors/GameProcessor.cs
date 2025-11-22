using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Services;
using TbspRpgProcessor.Entities;

namespace TbspRpgProcessor.Processors
{
    public interface IGameProcessor
    {
        Task<Game> StartGame(GameStartModel gameStartModel);
        Task RemoveGame(GameRemoveModel gameRemoveModel);
        Task RemoveGames(GamesRemoveModel gamesRemoveModel);
    }
    
    public class GameProcessor : IGameProcessor
    {
        private readonly IAdventuresService _adventuresService;
        private readonly IGamesService _gamesService;
        private readonly ILocationsService _locationsService;
        private readonly IContentsService _contentsService;
        private readonly IScriptProcessor _scriptProcessor;
        private readonly ILogger _logger;

        public GameProcessor(
            IScriptProcessor scriptProcessor,
            IAdventuresService adventuresService,
            IGamesService gamesService,
            ILocationsService locationsService,
            IContentsService contentsService,
            ILogger logger)
        {
            _scriptProcessor = scriptProcessor;
            _adventuresService = adventuresService;
            _gamesService = gamesService;
            _locationsService = locationsService;
            _contentsService = contentsService;
            _logger = logger;
        }
        
        public async Task<Game> StartGame(GameStartModel gameStartModel)
        {
            var adventure = await _adventuresService.GetAdventureById(gameStartModel.AdventureId);
            if (adventure == null)
                throw new ArgumentException("invalid adventure id");
            
            // check if the user already has a game of this adventure
            var game = await _gamesService.GetGameByAdventureId(adventure.Id);
            if (game != null)
                return game;
            
            // get the initial location for the game
            var location = await _locationsService.GetInitialLocationForAdventure(adventure.Id);
            if (location == null)
                throw new Exception("no initial location for adventure");
            
            // add game to the context
            var secondsSinceEpoch = new DateTimeOffset(gameStartModel.TimeStamp).ToUnixTimeMilliseconds();
            game = new Game()
            {
                AdventureId = adventure.Id,
                LocationId = location.Id,
                LocationUpdateTimeStamp = secondsSinceEpoch
            };
            await _gamesService.AddGame(game);
            
            // run the initialization script for the adventure if there is one
            if (adventure.InitializationScriptId != null)
            {
                await _scriptProcessor.ExecuteScript(new ScriptExecuteModel()
                {
                    ScriptId = adventure.InitializationScriptId.GetValueOrDefault(),
                    Game = game
                });
            }

            // create content entry for adventure's source key
            await _contentsService.AddContent(new Content()
            {
                GameId = game.Id,
                Position = (ulong)secondsSinceEpoch,
                SourceKey = adventure.InitialSourceKey
            });
            
            // create content entry for the initial location source key
            await _contentsService.AddContent(new Content()
            {
                GameId = game.Id,
                Position = (ulong)secondsSinceEpoch + 1,
                SourceKey = location.SourceKey
            });

            // save context changes
            await _gamesService.SaveChanges();
            return game;
        }

        public async Task RemoveGame(GameRemoveModel gameRemoveModel)
        {
            var game = await _gamesService.GetGameById(gameRemoveModel.GameId);
            if(game == null)
                throw new ArgumentException("invalid game id");

            await RemoveGame(game);
        }
        
        public async Task RemoveGames(GamesRemoveModel gamesRemoveModel)
        {
            foreach (var game in gamesRemoveModel.Games)
            {
                await RemoveGame(game, false);
            }

            if (gamesRemoveModel.Save)
                await _gamesService.SaveChanges();
        }
        
        private async Task RemoveGame(Game game, bool save = true)
        {
            // delete any associated content
            await _contentsService.RemoveAllContentsForGame(game.Id);
            // delete the game row
            _gamesService.RemoveGame(game);
            
            if(save)
                await _gamesService.SaveChanges();
        }
    }
}