using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.Entities;
using TbspRpgSettings.Settings;

namespace TbspRpgDataLayer.Repositories;

public interface IAdventureObjectSourceRepository
{
    Task<List<AdventureObjectSource>> GetAdventureObjectsWithSourceById(IEnumerable<int> adventureObjectIds, string language);
}

public class AdventureObjectSourceRepository: IAdventureObjectSourceRepository
{
    private readonly DatabaseContext _databaseContext;
    
    public AdventureObjectSourceRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public Task<List<AdventureObjectSource>> GetAdventureObjectsWithSourceById(IEnumerable<int> adventureObjectIds, string language)
    {
        var adventureObjectQueryable = _databaseContext.AdventureObjects.AsQueryable()
            .Where(ao => adventureObjectIds.Contains(ao.Id));
        IQueryable<AdventureObjectSource> adventureObjectSourceQueryable = null;
        if (language == null || language == Languages.ENGLISH)
        {
            adventureObjectSourceQueryable = adventureObjectQueryable
                .Join(
                    _databaseContext.SourcesEn, 
                    adventureObject => adventureObject.NameSourceKey,
                    sourceEn => sourceEn.Key,
                    (adventureObject, en) => new
                    {
                        AdventureObject = adventureObject,
                        NameSource = en
                    }
                ).Join(
                    _databaseContext.SourcesEn,
                    o => o.AdventureObject.DescriptionSourceKey,
                    sourceEn => sourceEn.Key,
                    (o, en) => new AdventureObjectSource
                    {
                        AdventureObject = o.AdventureObject,
                        NameSource = o.NameSource,
                        DescriptionSource = en
                    }
                );
        }
        else if (language == Languages.SPANISH)
        {
            adventureObjectSourceQueryable = adventureObjectQueryable
                .Join(
                    _databaseContext.SourcesEsp, 
                    adventureObject => adventureObject.NameSourceKey,
                    sourceEsp => sourceEsp.Key,
                    (adventureObject, esp) => new
                    {
                        AdventureObject = adventureObject,
                        NameSource = esp
                    }
                ).Join(
                    _databaseContext.SourcesEsp,
                    o => o.AdventureObject.DescriptionSourceKey,
                    sourceEsp => sourceEsp.Key,
                    (o, esp) => new AdventureObjectSource
                    {
                        AdventureObject = o.AdventureObject,
                        NameSource = o.NameSource,
                        DescriptionSource = esp
                    }
                );
        }

        if (adventureObjectSourceQueryable != null)
            return adventureObjectSourceQueryable.ToListAsync();
        
        throw new ArgumentException($"invalid language {language}");
    }
}