using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories;

public interface IAdventureObjectRepository: IBaseRepository
{
    Task<AdventureObject> GetAdventureObjectById(int adventureObjectId);
    Task<List<AdventureObject>> GetAdventureObjectsForAdventure(int adventureId);
    Task<List<AdventureObject>> GetAdventureObjectsByLocation(int locationId);
    Task AddAdventureObject(AdventureObject adventureObject);
    void RemoveAdventureObject(AdventureObject adventureObject);
    void RemoveAdventureObjects(ICollection<AdventureObject> adventureObjects);
    void AttachObject(AdventureObject adventureObject);
}

public class AdventureObjectRepository: IAdventureObjectRepository
{
    private readonly DatabaseContext _databaseContext;
    
    public AdventureObjectRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task SaveChanges()
    {
        await _databaseContext.SaveChangesAsync();
    }

    public Task<AdventureObject> GetAdventureObjectById(int adventureObjectId)
    {
        return _databaseContext.AdventureObjects.AsQueryable()
            .Include(ao => ao.Locations)
            .FirstOrDefaultAsync(ao => ao.Id == adventureObjectId);
    }

    public Task<List<AdventureObject>> GetAdventureObjectsForAdventure(int adventureId)
    {
        return _databaseContext.AdventureObjects.AsQueryable()
            .Where(ao => ao.AdventureId == adventureId)
            .Include(ao => ao.Locations)
            .ToListAsync();
    }

    public Task<List<AdventureObject>> GetAdventureObjectsByLocation(int locationId)
    {
        return _databaseContext.AdventureObjects.AsQueryable()
            .Where(ao => ao.Locations.Any(location => location.Id == locationId))
            .ToListAsync();
    }

    public async Task AddAdventureObject(AdventureObject adventureObject)
    {
        await _databaseContext.AdventureObjects.AddAsync(adventureObject);
    }
    
    public void RemoveAdventureObject(AdventureObject adventureObject)
    {
        _databaseContext.Remove(adventureObject);
    }

    public void RemoveAdventureObjects(ICollection<AdventureObject> adventureObjects)
    {
        _databaseContext.RemoveRange(adventureObjects);
    }

    public void AttachObject(AdventureObject adventureObject)
    {
        _databaseContext.Attach(adventureObject);
    }
}