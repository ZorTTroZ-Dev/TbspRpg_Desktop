using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLua.Exceptions;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor.Entities;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgProcessor.Tests.Processors;

public class ScriptProcessorTests: ProcessorTest
{
    #region ExecuteScript

    [Fact]
    public async Task ExecuteScript_NoIncludes_ScriptExecuted()
    {
        // arrange
        var script = new Script()
        {
            Id = 1,
            Name = "test script",
            Content = @"
                function banana()
                    return 'foo'
                end

                function run()
		            result = banana()
	            end
            ",
            Type = ScriptTypes.LuaScript
        };
        var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
            Scripts = new List<Script>() {script}
        });
        
        // act
        var result = await processor.ExecuteScript(new ScriptExecuteModel() {
            ScriptId = script.Id
        });
        
        // assert
        Assert.Equal("foo", result);
    }

    [Fact]
    public async Task ExecuteScript_WithIncludes_ScriptExecuted()
    {
        // arrange
        var script = new Script()
        {
            Id = 1,
            Name = "test script",
            Content = @"
                function run()
		            result = banana()
	            end
            ",
            Type = ScriptTypes.LuaScript,
            Includes = new List<Script>()
            {
                new()
                {
                    Id = 2,
                    Name = "base script",
                    Content = @"
                        function banana()
                            return 'foo'
                        end
                    ",
                    Type = ScriptTypes.LuaScript
                }
            }
        };
        var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
            Scripts = new List<Script>() {script}
        });
        
        // act
        var result = await processor.ExecuteScript(new ScriptExecuteModel() {
            ScriptId = script.Id
        });
        
        // assert
        Assert.Equal("foo", result);
    }
    
    [Fact]
    public async Task ExecuteScript_NoRunFunction_ExceptionThrown()
    {
        // arrange
        var script = new Script()
        {
            Id = 1,
            Name = "test script",
            Content = @"
                function banana()
                    return 'foo'
                end

                function fun()
		            result = banana()
	            end
            ",
            Type = ScriptTypes.LuaScript
        };
        var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
            Scripts = new List<Script>() {script}
        });
        
        // act
        Task Act() => processor.ExecuteScript(new ScriptExecuteModel() {
            ScriptId = script.Id
        });
        
        // assert
        await Assert.ThrowsAsync<LuaScriptException>(Act);
    }
    
    [Fact]
    public async Task ExecuteScript_BadScriptId_ExceptionThrown()
    {
        // arrange
        var script = new Script()
        {
            Id = 1,
            Name = "test script",
            Content = @"
                function banana()
                    return 'foo'
                end

                function fun()
		            result = banana()
	            end
            ",
            Type = ScriptTypes.LuaScript
        };
        var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
            Scripts = new List<Script>() {script}
        });
        
        // act
        Task Act() => processor.ExecuteScript(new ScriptExecuteModel() {
            ScriptId = 17
        });
        
        // assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }

    #endregion
    
    #region UpdateScript

    [Fact]
    public async Task UpdateScript_BadScriptId_ThrowException()
    {
        // arrange
        var testScript = new Script()
        {
            Id = 1,
            Name = "test script"
        };
        var scripts = new List<Script>() { testScript };
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Scripts = scripts
        });
        
        // act
        Task Act() => processor.UpdateScript(new ScriptUpdateModel()
        {
            script = new Script()
            {
                Id = 2,
                Name = "banana"
            }
        });
    
        // assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }
    
    [Fact]
    public async Task UpdateScript_EmptyScriptId_CreateNewScript()
    {
        // arrange
        var testScript = new Script()
        {
            Id = 1,
            Name = "test location",
            Type = ScriptTypes.LuaScript,
            Content = "function base()\n  result = 'banana'\nend",
            AdventureId = 1
        };
        var scripts = new List<Script>() { testScript };
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Scripts = scripts
        });
        
        // act
        await processor.UpdateScript(new ScriptUpdateModel()
        {
            script = new Script()
            {
                Name = "new script",
                Type = ScriptTypes.LuaScript,
                Content = "function run()\n  base()\n  result = 'banana'\nend",
                AdventureId = testScript.AdventureId,
                Includes = new List<Script>()
                {
                    testScript
                }
            }
        });
        
        // assert
        Assert.Equal(2, scripts.Count);
        Assert.Single(scripts[1].Includes);
    }
    
    [Fact]
    public async Task UpdateLocation_ScriptChange_ScriptUpdated()
    {
        // arrange
        var testScript = new Script()
        {
            Id = 1,
            Name = "test location",
            Type = ScriptTypes.LuaScript,
            Content = "function base()\n  result = 'banana'\nend",
            AdventureId = 1
        };
        var testScriptTwo = new Script()
        {
            Id = 2,
            Name = "test location two",
            Type = ScriptTypes.LuaScript,
            Content = "function\n  result = 'banana'\nend",
            AdventureId = 2
        };
        var scripts = new List<Script>() { testScript, testScriptTwo };
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Scripts = scripts
        });
        
        // act
        await processor.UpdateScript(new ScriptUpdateModel()
        {
            script = new Script()
            {
                Id = testScriptTwo.Id,
                Name = "new script",
                Type = ScriptTypes.LuaScript,
                Content = "function run()\n  base()\n  result = 'banana'\nend",
                AdventureId = testScript.AdventureId,
                Includes = new List<Script>()
                {
                    testScript
                }
            }
        });
        
        // assert
        Assert.Equal(2, scripts.Count);
        Assert.Single(scripts[1].Includes);
        Assert.Equal("new script", scripts[1].Name);
        Assert.Contains("base()", scripts[1].Content);
    }

    #endregion

    #region RemoveScript

    [Fact]
    public async Task RemoveScript_InvalidId_ExceptionThrown()
    {
        // arrange
        var testScript = new Script()
        {
            Id = 1,
            Name = "test script"
        };
        var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
            Scripts = new List<Script>() {testScript}
        });
        
        // act
        Task Act() => processor.RemoveScript(new ScriptRemoveModel()
        {
            ScriptId = 15
        });
    
        // assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }
    
    [Fact]
    public async Task RemoveScript_WithAdventures_ScriptRemoved()
    {
        // arrange
        var testScript = new Script()
        {
            Id = 1,
            Name = "test script"
        };
        var testAdventure = new Adventure()
        {
            Id = 1,
            Name = "test adventure",
            InitializationScriptId = testScript.Id,
            InitializationScript = testScript
        };
        var scripts = new List<Script>() { testScript };
        var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
            Scripts = scripts, 
            Adventures = new List<Adventure>() { testAdventure }
        });
        
        // act
        await processor.RemoveScript(new ScriptRemoveModel()
        {
            ScriptId = testScript.Id
        });
        
        // assert
        Assert.Empty(scripts);
        Assert.Null(testAdventure.InitializationScript);
    }

    [Fact]
    public async Task RemoveScript_WithRoutes_ScriptRemoved()
    {
        // arrange
        var testScript = new Script()
        {
            Id = 1,
            Name = "test script"
        };
        var testRoute = new Route()
        {
            Id = 1,
            Name = "test route",
            RouteTakenScriptId = testScript.Id,
            RouteTakenScript = testScript
        };
        var scripts = new List<Script>() { testScript };
        var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
            Scripts = scripts,
            Routes = new List<Route>() {testRoute}
        });
        
        // act
        await processor.RemoveScript(new ScriptRemoveModel()
        {
            ScriptId = testScript.Id
        });
        
        // assert
        Assert.Empty(scripts);
        Assert.Null(testRoute.RouteTakenScript);
    }

    [Fact]
    public async Task RemoveScript_WithLocations_ScriptRemoved()
    {
        // arrange
        var testScript = new Script()
        {
            Id = 1,
            Name = "test script"
        };
        var testLocation = new Location()
        {
            Id = 1,
            Name = "test location",
            EnterScript = testScript,
            EnterScriptId = testScript.Id
        };
        var scripts = new List<Script>() { testScript };
        var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
            Scripts = scripts,
            Locations = new List<Location>() { testLocation }
        });
        
        // act
        await processor.RemoveScript(new ScriptRemoveModel()
        {
            ScriptId = testScript.Id
        });
        
        // assert
        Assert.Empty(scripts);
        Assert.Null(testLocation.EnterScript);
    }

    [Fact]
    public async Task RemoveScript_WithSources_ScriptRemoved()
    {
        // arrange
        var testScript = new Script()
        {
            Id = 1,
            Name = "test script"
        };
        var testSource = new En()
        {
            Id = 1,
            Name = "test source",
            Script = testScript,
            ScriptId = testScript.Id
        };
        var scripts = new List<Script>() { testScript };
        var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
            Scripts = scripts,
            Sources = new List<En>() { testSource }
        });
        
        // act
        await processor.RemoveScript(new ScriptRemoveModel()
        {
            ScriptId = testScript.Id
        });
        
        // assert
        Assert.Empty(scripts);
        Assert.Null(testSource.Script);
    }

    [Fact]
    public async Task RemoveScript_WithIncludes_ScriptRemoved()
    {
        // arrange
        var includeScript = new Script()
        {
            Id = 1,
            Name = "include script"
        };
        var testScript = new Script()
        {
            Id = 2,
            Name = "test script",
            IncludedIn = new List<Script>() { includeScript }
        };
        var scripts = new List<Script>() { testScript, includeScript };
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Scripts = scripts
        });
        
        // act
        await processor.RemoveScript(new ScriptRemoveModel()
        {
            ScriptId = testScript.Id
        });
        
        // assert
        Assert.Single(scripts);
        Assert.Empty(testScript.IncludedIn);
    }

    #endregion
}