using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories
{
    public interface IContentsRepository : IBaseRepository
    {
        Task AddContent(Content content);
        Task<Content> GetContentForGameAtPosition(int gameId, ulong position);
        Task<List<Content>> GetContentForGame(int gameId, int? offset = null, int? count = null);
        Task<List<Content>> GetContentForGameReverse(int gameId, int? offset = null, int? count = null);
        Task<List<Content>> GetContentForGameAfterPosition(int gameId, ulong position);
        void RemoveContents(IEnumerable<Content> contents);
        Task RemoveAllContentsForGame(int gameId);
        Task<List<Content>> GetAdventureContentsWithSource(int adventureId, Guid sourceKey);
    }
    
    public class ContentsRepository : IContentsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ContentsRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task AddContent(Content content)
        {
            await _databaseContext.AddAsync(content);
        }

        public Task<Content> GetContentForGameAtPosition(int gameId, ulong position)
        {
            return _databaseContext.Contents.AsQueryable()
                .FirstOrDefaultAsync(content => content.GameId == gameId && content.Position == position);
        }

        public Task<List<Content>> GetContentForGame(int gameId, int? offset = null, int? count = null)
        {
            var query = _databaseContext.Contents.AsQueryable()
                .Where(c => c.GameId == gameId)
                .OrderBy(c => c.Position);
            if (offset != null)
                query = (IOrderedQueryable<Content>) query.Skip(offset.Value);
            if (count != null)
                query = (IOrderedQueryable<Content>) query.Take(count.Value);
            return query.ToListAsync();
        }

        public Task<List<Content>> GetContentForGameReverse(int gameId, int? offset = null, int? count = null)
        {
            var query = _databaseContext.Contents.AsQueryable()
                .Where(c => c.GameId == gameId)
                .OrderByDescending(c => c.Position);
            if (offset != null)
                query = (IOrderedQueryable<Content>) query.Skip(offset.Value);
            if (count != null)
                query = (IOrderedQueryable<Content>) query.Take(count.Value);
            return query.ToListAsync();
        }

        public Task<List<Content>> GetContentForGameAfterPosition(int gameId, ulong position)
        {
            return _databaseContext.Contents.AsQueryable()
                .Where(c => c.GameId == gameId && c.Position > position)
                .OrderBy(c => c.Position).ToListAsync();
        }

        public void RemoveContents(IEnumerable<Content> contents)
        {
            _databaseContext.RemoveRange(contents);
        }

        public async Task RemoveAllContentsForGame(int gameId)
        {
            var contents = await GetContentForGame(gameId);
            RemoveContents(contents);
        }

        public Task<List<Content>> GetAdventureContentsWithSource(int adventureId, Guid sourceKey)
        {
            return _databaseContext.Contents.AsQueryable()
                .Where(content => content.Game.AdventureId == adventureId && content.SourceKey == sourceKey)
                .ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}