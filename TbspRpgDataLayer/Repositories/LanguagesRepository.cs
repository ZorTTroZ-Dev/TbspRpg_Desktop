using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories;

public interface ILanguagesRepository: IBaseRepository
{
    Task<Language> GetLanguagesById(int languageId);
    Task<List<Language>> GetAllLanguagesAsync();
    List<Language> GetAllLanguages();
    Language GetDefaultLanguage();
    void Seed();
}

public class LanguagesRepository: ILanguagesRepository
{
    private readonly DatabaseContext _databaseContext;
    
    public LanguagesRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task SaveChanges()
    {
        await _databaseContext.SaveChangesAsync();
    }

    public Task<Language> GetLanguagesById(int languageId)
    {
        return _databaseContext.Languages.AsQueryable().FirstOrDefaultAsync(l => l.Id == languageId);
    }

    public Task<List<Language>> GetAllLanguagesAsync()
    {
        return _databaseContext.Languages.AsQueryable().ToListAsync();
    }

    public List<Language> GetAllLanguages()
    {
        return _databaseContext.Languages.ToList();
    }

    public Language GetDefaultLanguage()
    {
        return _databaseContext.Languages.FirstOrDefault(lang => lang.Code == "en");
    }

    public void Seed()
    {
        var english = new Language()
        {
            Name = "English",
            Code = "en"
        };
        var spanish = new Language()
        {
            Name = "Spanish",
            Code = "es"
        };
        _databaseContext.Languages.Add(english);
        _databaseContext.Languages.Add(spanish);
        _databaseContext.SaveChanges();
    }
}