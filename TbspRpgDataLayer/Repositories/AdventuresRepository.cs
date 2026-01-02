using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories
{
    public interface IAdventuresRepository: IBaseRepository
    {
        Task<List<Adventure>> GetAllAdventures(AdventureFilter filters);
        Task<Adventure> GetAdventureByName(string name);
        Task<Adventure> GetAdventureById(int adventureId);
        Task<Adventure> GetAdventureByIdIncludeAssociatedObjects(int adventureId);
        Task AddAdventure(Adventure adventure);
        void RemoveAdventure(Adventure adventure);
        Task<List<Adventure>> GetAdventuresWithScript(int scriptId);
        Task<Adventure> GetAdventureWithSource(int adventureId, Guid sourceKey);
    }
    
    public class AdventuresRepository : IAdventuresRepository
    {
        private readonly DatabaseContext _databaseContext;

        public AdventuresRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        private IQueryable<Adventure> GetFilteredQuery(AdventureFilter filters)
        {
            var query = _databaseContext.Adventures.AsQueryable();
            return query;
        }

        public Task<List<Adventure>> GetAllAdventures(AdventureFilter filters)
        {
            return GetFilteredQuery(filters).ToListAsync();
        }

        public Task<Adventure> GetAdventureByName(string name)
        {
            return _databaseContext.Adventures.AsQueryable().
                Where(a => a.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();
        }

        public Task<Adventure> GetAdventureById(int adventureId)
        {
            return _databaseContext.Adventures.AsQueryable().
                Where(a => a.Id == adventureId).
                FirstOrDefaultAsync();
        }

        public Task<Adventure> GetAdventureByIdIncludeAssociatedObjects(int adventureId)
        {
            return _databaseContext.Adventures.AsQueryable()
                .Include(adventure => adventure.Games)
                .Include(adventure => adventure.Locations)
                .ThenInclude(location => location.Routes)
                .Where(adventure => adventure.Id == adventureId)
                .FirstOrDefaultAsync();
        }

        public async Task AddAdventure(Adventure adventure)
        {
            await _databaseContext.AddAsync(adventure);
        }

        public void RemoveAdventure(Adventure adventure)
        {
            _databaseContext.Remove(adventure);
        }

        public Task<List<Adventure>> GetAdventuresWithScript(int scriptId)
        {
            return _databaseContext.Adventures.AsQueryable()
                .Where(adventure =>
                    adventure.TerminationScriptId == scriptId || adventure.InitializationScriptId == scriptId)
                .ToListAsync();
        }

        public Task<Adventure> GetAdventureWithSource(int adventureId, Guid sourceKey)
        {
            return _databaseContext.Adventures.AsQueryable()
                .FirstOrDefaultAsync(adventure =>
                    adventure.Id == adventureId &&
                    (adventure.DescriptionCopyKey == sourceKey 
                     || adventure.InitialCopyKey == sourceKey));
        }

        public async Task SaveChanges()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}