using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories;

public interface IScriptsRepository: IBaseRepository
{
    Task<Script> GetScriptById(int scriptId);
    Task<List<Script>> GetScriptsForAdventure(int adventureId);
    Task AddScript(Script script);
    void RemoveScript(Script script);
    void RemoveScripts(ICollection<Script> scripts);
    void AttachScript(Script script);
    Task<List<Script>> GetAdventureScriptsWithSourceReference(int adventureId, Guid sourceKey);
    Task<Script> GetScriptForAdventureWithName(int adventureId, string scriptName);
}

public class ScriptsRepository: IScriptsRepository
{
    private readonly DatabaseContext _databaseContext;

    public ScriptsRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public Task<Script> GetScriptById(int scriptId)
    {
        return _databaseContext.Scripts.FirstOrDefaultAsync(script => script.Id == scriptId);
    }

    public Task<List<Script>> GetScriptsForAdventure(int adventureId)
    {
        return _databaseContext.Scripts.AsQueryable()
            .Where(script => script.AdventureId == adventureId)
            .ToListAsync();
    }

    public async Task AddScript(Script script)
    {
        await _databaseContext.AddAsync(script);
    }

    public void RemoveScript(Script script)
    {
        _databaseContext.Remove(script);
    }
    
    public void RemoveScripts(ICollection<Script> scripts)
    {
        _databaseContext.RemoveRange(scripts);
    }

    public void AttachScript(Script script)
    {
        _databaseContext.Attach(script);
    }

    public Task<List<Script>> GetAdventureScriptsWithSourceReference(int adventureId, Guid sourceKey)
    {
        return _databaseContext.Scripts.AsQueryable()
            .Where(script => script.AdventureId == adventureId && script.Content.Contains(sourceKey.ToString()))
            .ToListAsync();
    }

    public Task<Script> GetScriptForAdventureWithName(int adventureId, string scriptName)
    {
        return _databaseContext.Scripts.AsQueryable()
            .Where(script => script.AdventureId == adventureId && script.Name == scriptName)
            .FirstOrDefaultAsync();
    }

    public async Task SaveChanges()
    {
        await _databaseContext.SaveChangesAsync();
    }
}