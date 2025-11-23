using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services;

public interface IAdventureObjectService : IBaseService
{
    Task<AdventureObject> GetAdventureObjectById(int adventureObjectId);
    Task<List<AdventureObject>> GetAdventureObjectsForAdventure(int adventureId);
    Task<List<AdventureObject>> GetAdventureObjectsByLocation(int locationId);
    Task AddAdventureObject(AdventureObject adventureObject);
    void RemoveAdventureObject(AdventureObject adventureObject);
    void RemoveAdventureObjects(ICollection<AdventureObject> adventureObjects);
    void AttachObject(AdventureObject adventureObject);
}

public class AdventureObjectService: IAdventureObjectService
{
    private readonly IAdventureObjectRepository _adventureObjectRepository;
    private readonly ILogger<AdventureObjectService> _logger;
    
    public AdventureObjectService(IAdventureObjectRepository adventureObjectRepository,
        ILogger<AdventureObjectService> logger)
    {
        _adventureObjectRepository = adventureObjectRepository;
        _logger = logger;
    }
    
    public async Task SaveChanges()
    {
        await _adventureObjectRepository.SaveChanges();
    }

    public Task<AdventureObject> GetAdventureObjectById(int adventureObjectId)
    {
        return _adventureObjectRepository.GetAdventureObjectById(adventureObjectId);
    }

    public Task<List<AdventureObject>> GetAdventureObjectsForAdventure(int adventureId)
    {
        return _adventureObjectRepository.GetAdventureObjectsForAdventure(adventureId);
    }

    public Task<List<AdventureObject>> GetAdventureObjectsByLocation(int locationId)
    {
        return _adventureObjectRepository.GetAdventureObjectsByLocation(locationId);
    }

    public async Task AddAdventureObject(AdventureObject adventureObject)
    {
        await _adventureObjectRepository.AddAdventureObject(adventureObject);
    }

    public void RemoveAdventureObject(AdventureObject adventureObject)
    {
        _adventureObjectRepository.RemoveAdventureObject(adventureObject);
    }

    public void RemoveAdventureObjects(ICollection<AdventureObject> adventureObjects)
    {
        _adventureObjectRepository.RemoveAdventureObjects(adventureObjects);
    }

    public void AttachObject(AdventureObject adventureObject)
    {
        _adventureObjectRepository.AttachObject(adventureObject);
    }
}