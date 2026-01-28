using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer.Repositories;

public interface IScriptIncludesRepository: IBaseRepository
{
    Task<List<ScriptInclude>> GetIncludesForScriptId(int scriptId);
    Task<List<ScriptInclude>> GetIncludedInForScriptId(int scriptId);
    void RemoveScriptIncludes(int scriptId);
    Task<ScriptInclude> GetScriptInclude(int includedInId, int includesId);
    Task AddScriptInclude(ScriptInclude scriptInclude);
}

public class ScriptIncludesRepository: IScriptIncludesRepository
{
    private readonly DatabaseContext _databaseContext;

    public ScriptIncludesRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    

    public async Task SaveChanges()
    {
        await _databaseContext.SaveChangesAsync();
    }

    public Task<List<ScriptInclude>> GetIncludesForScriptId(int scriptId)
    {
        return _databaseContext.ScriptIncludes.AsQueryable()
            .Include(si => si.Includes)
            .Include(si => si.IncludedIn)
            .Where(si => si.IncludedInId == scriptId)
            .OrderBy(si => si.Order)
            .ToListAsync();
    }

    public Task<List<ScriptInclude>> GetIncludedInForScriptId(int scriptId)
    {
        return _databaseContext.ScriptIncludes.AsQueryable()
            .Include(si => si.Includes)
            .Include(si => si.IncludedIn)
            .Where(si => si.IncludesId == scriptId)
            .ToListAsync();
    }

    public void RemoveScriptIncludes(int scriptId)
    {
        var includes = _databaseContext.ScriptIncludes.AsQueryable()
            .Where(si => si.IncludedInId == scriptId || si.IncludesId == scriptId)
            .ToList();
        foreach (var include in includes)
        {
            _databaseContext.ScriptIncludes.Remove(include);
        }
    }

    public Task<ScriptInclude> GetScriptInclude(int includedInId, int includesId)
    {
        return _databaseContext.ScriptIncludes.AsQueryable()
            .FirstOrDefaultAsync(si => si.IncludedInId == includedInId && si.IncludesId == includesId);
    }

    public async Task AddScriptInclude(ScriptInclude scriptInclude)
    {
        await _databaseContext.ScriptIncludes.AddAsync(scriptInclude);
    }
}