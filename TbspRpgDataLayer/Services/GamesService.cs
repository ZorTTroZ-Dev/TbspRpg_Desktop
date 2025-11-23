using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services
{
    public interface IGamesService : IBaseService
    {
        Task<Game> GetGameByAdventureId(int adventureId);
        Task AddGame(Game game);
        Task<Game> GetGameByIdIncludeLocation(int gameId);
        Task<Game> GetGameById(int gameId);
        Task<Game> GetGameByIdIncludeAdventure(int gameId);
        Task<List<Game>> GetGamesByAdventureId(int adventureId);
        Task<List<Game>> GetGames(GameFilter filters);
        Task<List<Game>> GetGamesIncludeUsers(GameFilter filters);
        void RemoveGame(Game game);
        void RemoveGames(ICollection<Game> games);
    }
    
    public class GamesService : IGamesService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GamesService> _logger;
        
        public GamesService(IGameRepository gameRepository,
            ILogger<GamesService> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public Task<Game> GetGameByAdventureId(int adventureId)
        {
            return _gameRepository.GetGameByAdventureId(adventureId);
        }

        public async Task AddGame(Game game)
        {
            await _gameRepository.AddGame(game);
        }

        public Task<Game> GetGameByIdIncludeLocation(int gameId)
        {
            return _gameRepository.GetGameByIdWithLocation(gameId);
        }

        public Task<Game> GetGameById(int gameId)
        {
            return _gameRepository.GetGameById(gameId);
        }
        
        public Task<Game> GetGameByIdIncludeAdventure(int gameId)
        {
            return _gameRepository.GetGameByIdWithAdventure(gameId);
        }

        public Task<List<Game>> GetGamesByAdventureId(int adventureId)
        {
            return GetGames(new GameFilter()
            {
                AdventureId = adventureId
            });
        }

        public Task<List<Game>> GetGames(GameFilter filters)
        {
            return _gameRepository.GetGames(filters);
        }

        public Task<List<Game>> GetGamesIncludeUsers(GameFilter filters)
        {
            return _gameRepository.GetGames(filters);
        }

        public void RemoveGame(Game game)
        {
            _gameRepository.RemoveGame(game);
        }

        public void RemoveGames(ICollection<Game> games)
        {
            _gameRepository.RemoveGames(games);
        }

        public async Task SaveChanges()
        {
            await _gameRepository.SaveChanges();
        }
    }
}