using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories
{
    public interface IGameRepository : IBaseRepository
    {
        Task<Game> GetGameById(int gameId);
        Task<Game> GetGameByIdWithAdventure(int gameId);
        Task<Game> GetGameByIdWithLocation(int gameId);
        Task<Game> GetGameByAdventureId(int adventureId);
        Task<List<Game>> GetGames(GameFilter filters);
        Task AddGame(Game game);
        void RemoveGame(Game game);
        void RemoveGames(ICollection<Game> games);
    }
    
    public class GameRepository : IGameRepository
    {
        private readonly DatabaseContext _databaseContext;

        public GameRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Task<Game> GetGameById(int gameId)
        {
            return _databaseContext.Games.AsQueryable()
                .FirstOrDefaultAsync(g => g.Id == gameId);
        }
        
        public Task<Game> GetGameByIdWithAdventure(int gameId)
        {
            return _databaseContext.Games.AsQueryable()
                .Include(g => g.Adventure)
                .FirstOrDefaultAsync(g => g.Id == gameId);
        }

        public Task<Game> GetGameByIdWithLocation(int gameId)
        {
            return _databaseContext.Games.AsQueryable().Include(g => g.Location)
                .FirstOrDefaultAsync(g => g.Id == gameId);
        }

        public Task<Game> GetGameByAdventureId(int adventureId)
        {
            return _databaseContext.Games.AsQueryable()
                .FirstOrDefaultAsync(g => g.AdventureId == adventureId);
        }

        private IQueryable<Game> BuildGamesQuery(GameFilter filters)
        {
            var query = _databaseContext.Games.AsQueryable();
            if (filters.AdventureId != null)
                query = query.Where(game => game.AdventureId == filters.AdventureId);
            return query;
        }

        public Task<List<Game>> GetGames(GameFilter filters)
        {
            if (filters == null) return _databaseContext.Games.AsQueryable().ToListAsync();
            var query = BuildGamesQuery(filters);
            return query.ToListAsync();
        }

        public async Task AddGame(Game game)
        {
            await _databaseContext.Games.AddAsync(game);
        }

        public void RemoveGame(Game game)
        {
            _databaseContext.Remove(game);
        }

        public void RemoveGames(ICollection<Game> games)
        {
            _databaseContext.RemoveRange(games);
        }

        public async Task SaveChanges()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}