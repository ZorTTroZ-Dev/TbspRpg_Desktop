using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services;

public interface IScriptIncludesService: IBaseService
{
    Task<List<ScriptInclude>> GetIncludesForScriptId(int scriptId);
    Task<List<ScriptInclude>> GetIncludedInForScriptId(int scriptId);
    Task<ScriptInclude> GetScriptInclude(int includedInId, int includesId);
    void RemoveScriptIncludes(int scriptId);
    Task AddScriptInclude(ScriptInclude scriptInclude);
    Task<List<ScriptInclude>> CollectAllIncludes(int scriptId);
}

public class ScriptIncludesService: IScriptIncludesService
{
    private readonly IScriptIncludesRepository _scriptIncludesRepository;
    private readonly ILogger<ScriptIncludesService> _logger;

    public ScriptIncludesService(IScriptIncludesRepository scriptIncludesRepository,
        ILogger<ScriptIncludesService> logger)
    {
        _scriptIncludesRepository = scriptIncludesRepository;
        _logger = logger;
    }
    
    public async Task SaveChanges()
    {
        await _scriptIncludesRepository.SaveChanges();
    }

    public Task<List<ScriptInclude>> GetIncludesForScriptId(int scriptId)
    {
        return _scriptIncludesRepository.GetIncludesForScriptId(scriptId);
    }

    public Task<List<ScriptInclude>> GetIncludedInForScriptId(int scriptId)
    {
        return _scriptIncludesRepository.GetIncludedInForScriptId(scriptId);
    }

    public Task<ScriptInclude> GetScriptInclude(int includedInId, int includesId)
    {
        return _scriptIncludesRepository.GetScriptInclude(includedInId, includesId);
    }

    public void RemoveScriptIncludes(int scriptId)
    {
        _scriptIncludesRepository.RemoveScriptIncludes(scriptId);
    }

    public Task AddScriptInclude(ScriptInclude scriptInclude)
    {
        return _scriptIncludesRepository.AddScriptInclude(scriptInclude);
    }

    private async Task IncludeIncludes(int scriptId, List<ScriptInclude> allIncludes)
    {
        var includes = await GetIncludesForScriptId(scriptId);
        foreach (var include in includes)
        {
            await IncludeIncludes(include.IncludesId, allIncludes);
            if(!allIncludes.Exists(inc => inc.IncludesId == include.IncludesId))
                allIncludes.Add(include);
        }
    }

    public async Task<List<ScriptInclude>> CollectAllIncludes(int scriptId)
    {
        var allIncludes = new List<ScriptInclude>();
        await IncludeIncludes(scriptId, allIncludes);
        return allIncludes;
    }
}