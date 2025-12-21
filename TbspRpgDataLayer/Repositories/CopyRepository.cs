using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories;

public interface ICopyRepository: IBaseRepository
{
    void Seed(List<Language> languages);
    Task<Copy> GetCopyForKeyAndLanguage(Guid key, int languageId);
    Task AddCopy(Copy copy);
}

public class CopyRepository: ICopyRepository
{
    private readonly DatabaseContext _databaseContext;
    
    public CopyRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task SaveChanges()
    {
        await _databaseContext.SaveChangesAsync();
    }

    public void Seed(List<Language> languages)
    {
        foreach (var language in languages)
        {
            var emptyCopy = _databaseContext.Copy
                .FirstOrDefault(copy => copy.Key == Guid.Empty && copy.Language.Id ==  language.Id);
            if (emptyCopy == null)
            {
                var copy = new Copy()
                {
                    Key = Guid.Empty,
                    AdventureId = null,
                    Name = "empty",
                    Text = "empty copy",
                    Language = language
                };
                _databaseContext.Copy.Add(copy);
            }
        }
        _databaseContext.SaveChanges();
    }

    public Task<Copy> GetCopyForKeyAndLanguage(Guid key, int languageId) {
        return _databaseContext.Copy
            .Where(copy => copy.Key == key && copy.Language.Id == languageId)
            .FirstOrDefaultAsync();
    }

    public async Task AddCopy(Copy copy)
    {
        await _databaseContext.Copy.AddAsync(copy);
    }
}