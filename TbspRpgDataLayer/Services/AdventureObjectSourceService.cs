using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services;

public interface IAdventureObjectSourceService
{
    Task<List<AdventureObjectSource>> GetAdventureObjectsWithSourceById(IEnumerable<int> adventureObjectIds, string language);
}

public class AdventureObjectSourceService: IAdventureObjectSourceService
{
    private readonly IAdventureObjectSourceRepository _adventureObjectSourceRepository;
    private readonly ILogger<AdventureObjectSourceService> _logger;
    
    public AdventureObjectSourceService(IAdventureObjectSourceRepository adventureObjectSourceRepository,
        ILogger<AdventureObjectSourceService> logger)
    {
        _adventureObjectSourceRepository = adventureObjectSourceRepository;
        _logger = logger;
    }
    
    public Task<List<AdventureObjectSource>> GetAdventureObjectsWithSourceById(IEnumerable<int> adventureObjectIds, string language)
    {
        return _adventureObjectSourceRepository.GetAdventureObjectsWithSourceById(adventureObjectIds, language);
    }
}