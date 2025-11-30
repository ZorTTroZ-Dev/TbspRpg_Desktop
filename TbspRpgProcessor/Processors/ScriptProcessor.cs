using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NLua;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Services;
using TbspRpgProcessor.Entities;
using TbspRpgSettings;
using TbspRpgSettings.Settings;

namespace TbspRpgProcessor.Processors;

public interface IScriptProcessor
{
    Task<string> ExecuteScript(ScriptExecuteModel scriptExecuteModel);
    Task RemoveScript(ScriptRemoveModel scriptIdRemoveModel);
    Task UpdateScript(ScriptUpdateModel scriptUpdateModel);
    Task<Script> CreateScript(ScriptCreateModel scriptCreateModel);
}

public class ScriptProcessor : IScriptProcessor
{
    private readonly IScriptsService _scriptsService;
    private readonly IAdventuresService _adventuresService;
    private readonly IRoutesService _routesService;
    private readonly ILocationsService _locationsService;
    private readonly ISourcesService _sourcesService;
    private readonly IGamesService _gamesService;
    private readonly ILogger _logger;

    public ScriptProcessor(IScriptsService scriptsService,
        IAdventuresService adventuresService,
        IRoutesService routesService,
        ILocationsService locationsService,
        ISourcesService sourcesService,
        IGamesService gamesService,
        ILogger logger)
    {
        _scriptsService = scriptsService;
        _adventuresService = adventuresService;
        _routesService = routesService;
        _locationsService = locationsService;
        _sourcesService = sourcesService;
        _gamesService = gamesService;
        _logger = logger;
    }
    
    // Execute Script Method
    // Crete the Lua environment
    // Sandbox the environment, https://github.com/kikito/lua-sandbox
    // load the game state from the database, future addition
    // Load any scripts included by the script
    // add the script to the environment
    // execute the script
    // update the game state in the database, future addition
    // retrieve the result of the script
        
    // I've also been thinking of adding variables to the content that can be replaced in the
    // associated source. So when content added, provide a source key and variable values that
    // would get replaced when source rendered.
    private async Task<Script> VerifyScriptId(int scriptId)
    {
        if(scriptId <= TbspRpgUtilities.DB_EMPTY_ID)
            throw new ArgumentException("invalid script id");
            
        var dbScript = await _scriptsService.GetScriptById(scriptId);
        if (dbScript == null)
            throw new ArgumentException("invalid script id");
        
        return dbScript;
    }

    public async Task<string> ExecuteScript(ScriptExecuteModel scriptExecuteModel)
    {
        // could have just a script id
        // or a game id with a script id
        // or a game id with a script object
        // or a game with a script id
        // or a game with a script object
        if (scriptExecuteModel.Game == null && scriptExecuteModel.GameId != TbspRpgUtilities.DB_EMPTY_ID)
        {
            var dbGame = await _gamesService.GetGameById(scriptExecuteModel.GameId);
            if (dbGame == null)
                throw new ArgumentException("invalid game id");
            scriptExecuteModel.Game = dbGame;
        }

        if (scriptExecuteModel.Script == null)
        {
            scriptExecuteModel.Script = await VerifyScriptId(scriptExecuteModel.ScriptId);
        }
        
        var luaState = new Lua();
        
        // load sandbox lua library
        luaState["sandbox"] = luaState.DoString(LuaSandbox.LuaSandboxCode).First();
        
        // add the game as a global variable if it exists
        if (scriptExecuteModel.Game != null)
        {
            luaState["game"] = scriptExecuteModel.Game;
        }
        
        // load any includes
        if (scriptExecuteModel.Script.Includes != null)
        {
            foreach (var include in scriptExecuteModel.Script.Includes)
            {
                luaState.DoString(include.Content);
            }
        }

        // load the script
        luaState.DoString(scriptExecuteModel.Script.Content);
        
        // have to put the run function in the environment or it won't run
        luaState.DoString("sandbox_run = sandbox('run()', {env = { run = run }})");
        
        // run the script
        var scriptFunc = luaState["sandbox_run"] as LuaFunction;
        if (scriptFunc == null) return null;
        scriptFunc.Call();
        return luaState["result"] as string;
    }

    public async Task RemoveScript(ScriptRemoveModel scriptRemoveModel)
    {
        var dbScript = await _scriptsService.GetScriptWithIncludedIn(scriptRemoveModel.ScriptId);
        if (dbScript == null)
        {
            throw new ArgumentException("invalid script id");
        }
        
        // need to check if script attached to an adventure, location, route or source
        await _adventuresService.RemoveScriptFromAdventures(scriptRemoveModel.ScriptId);
        await _routesService.RemoveScriptFromRoutes(scriptRemoveModel.ScriptId);
        await _locationsService.RemoveScriptFromLocations(scriptRemoveModel.ScriptId);
        await _sourcesService.RemoveScriptFromSources(scriptRemoveModel.ScriptId);
        
        // remove this script from being included in other scripts
        dbScript.IncludedIn = new List<Script>();
        
        // then delete the script
        _scriptsService.RemoveScript(dbScript);

        await _scriptsService.SaveChanges();
    }

    public async Task<Script> CreateScript(ScriptCreateModel scriptCreateModel)
    {
        // create a new script
        var script = new Script()
        {
            AdventureId = scriptCreateModel.script.AdventureId,
            Name = scriptCreateModel.script.Name,
            Type = scriptCreateModel.script.Type,
            Content = scriptCreateModel.script.Content,
            Includes = new List<Script>()
        };
        foreach (var include in scriptCreateModel.script.Includes)
        {
            _scriptsService.AttachScript(include);
            script.Includes.Add(include);
        }
        await _scriptsService.AddScript(script);
        if (scriptCreateModel.Save)
            await _scriptsService.SaveChanges();
        return script;
    }

    public async Task UpdateScript(ScriptUpdateModel scriptUpdateModel)
    {
        if (scriptUpdateModel.script.Id == TbspRpgUtilities.DB_EMPTY_ID)
        {
            var script = await CreateScript(new ScriptCreateModel()
            {
                script = scriptUpdateModel.script,
                Save = false
            });
        }
        else
        {
            // update existing script
            var dbScript = await _scriptsService.GetScriptById(scriptUpdateModel.script.Id);
            if (dbScript == null)
                throw new ArgumentException("invalid script id");
            dbScript.Name = scriptUpdateModel.script.Name;
            dbScript.Type = scriptUpdateModel.script.Type;
            dbScript.Content = scriptUpdateModel.script.Content;
            dbScript.Includes = scriptUpdateModel.script.Includes;
        }

        await _scriptsService.SaveChanges();
    }
}