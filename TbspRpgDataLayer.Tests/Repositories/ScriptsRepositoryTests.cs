using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgDataLayer.Tests.Repositories;

public class ScriptsRepositoryTests() : InMemoryTest("ScriptsRepositoryTests")
{
    #region GetScriptById

    [Fact]
    public async Task GetScriptById_InvalidId_ReturnNull()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test",
            Content = "print('banana');",
            Type = ScriptTypes.LuaScript,
            Includes = new List<Script>()
        };
        await using var context = new DatabaseContext(DbContextOptions);
        context.Scripts.Add(testScript);
        await context.SaveChangesAsync();
        var repository = new ScriptsRepository(context);

        // act
        var script = await repository.GetScriptById(687);

        // assert
        Assert.Null(script);
    }

    [Fact]
    public async Task GetScriptById_Valid_ReturnScriptIncludeIncludes()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test",
            Content = "print('banana');",
            Type = ScriptTypes.LuaScript,
            Includes = new List<Script>()
            {
                new()
                {
                    Name = "test_base",
                    Content = "print('base banana');",
                    Type = ScriptTypes.LuaScript,
                }
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        context.Scripts.Add(testScript);
        await context.SaveChangesAsync();
        var repository = new ScriptsRepository(context);

        // act
        var script = await repository.GetScriptById(testScript.Id);
        
        // assert
        Assert.NotNull(script);
        Assert.Equal(testScript.Id, script.Id);
        Assert.Single(script.Includes);
    }

    #endregion

    #region GetScriptsForAdventure

    [Fact]
    public async Task GetScriptsForAdventure_ValidId_ReturnsScripts()
    {
        // arrange
        var testScripts = new List<Script>()
        {
            new()
            {
                Adventure = new Adventure()
                {
                    Name = "test"
                }
            },
            new()
            {
                Adventure = new Adventure()
                {
                    Name = "test two"
                }
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Scripts.AddRangeAsync(testScripts);
        await context.SaveChangesAsync();
        var repository = new ScriptsRepository(context);
        
        // act
        var scripts = await repository.GetScriptsForAdventure(testScripts[0].Adventure.Id);
        
        // assert
        Assert.Single(scripts);
        Assert.Equal("test", scripts[0].Adventure.Name);
    }

    [Fact]
    public async Task GetScriptsForAdventure_InvalidId_ReturnEmptyList()
    {
        // arrange
        var testScripts = new List<Script>()
        {
            new()
            {
                Adventure = new Adventure()
                {
                    Name = "test"
                }
            },
            new()
            {
                Adventure = new Adventure()
                {
                    Name = "test two"
                }
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Scripts.AddRangeAsync(testScripts);
        await context.SaveChangesAsync();
        var repository = new ScriptsRepository(context);
        
        // act
        var scripts = await repository.GetScriptsForAdventure(367);
        
        // assert
        Assert.Empty(scripts);
    }

    #endregion
    
    #region AddScript

    [Fact]
    public async Task AddScript_ScriptAdded()
    {
        // arrange
        await using var context = new DatabaseContext(DbContextOptions);
        var testScript = new Script()
        {
            Name = "test script"
        };
        var repository = new ScriptsRepository(context);
        
        // act
        await repository.AddScript(testScript);
        await repository.SaveChanges();
        
        // assert
        Assert.Single(context.Scripts);
    }

        #endregion
        
    #region RemoveScript

    [Fact]
    public async Task RemoveScript_ScriptRemoved()
    {
        // arrange
        await using var context = new DatabaseContext(DbContextOptions);
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptTwo = new Script()
        {
            Name = "test script Two"
        };
        context.Scripts.Add(testScript);
        context.Scripts.Add(testScriptTwo);
        await context.SaveChangesAsync();
        var repository = new ScriptsRepository(context);

        // act
        repository.RemoveScript(testScript);
        await repository.SaveChanges();
        
        // assert
        Assert.Single(context.Scripts);
    }
    
    #endregion

    #region RemoveScripts

    [Fact]
    public async Task RemoveScripts_ScriptsRemoved()
    {
        // arrange
        await using var context = new DatabaseContext(DbContextOptions);
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptTwo = new Script()
        {
            Name = "test script Two"
        };
        context.Scripts.Add(testScript);
        context.Scripts.Add(testScriptTwo);
        await context.SaveChangesAsync();
        var repository = new ScriptsRepository(context);
        
        // act
        repository.RemoveScripts(new List<Script>() { testScript, testScriptTwo});
        await repository.SaveChanges();
        
        // assert
        Assert.Empty(context.Scripts);
    }

    #endregion

    #region RemoveIncludes

    [Fact]
    public async Task RemoveIncludes_IncludesRemoved()
    {
        // arrange
        await using var context = new DatabaseContext(DbContextOptions);
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptTwo = new Script()
        {
            Name = "test script Two",
            Includes = new List<Script>() { testScript }
        };
        context.Scripts.Add(testScript);
        context.Scripts.Add(testScriptTwo);
        await context.SaveChangesAsync();
        var repository = new ScriptsRepository(context);
        
        // act
        var script = context.Scripts.First(script => script.Id == testScriptTwo.Id);
        script.Includes = new List<Script>();
        await repository.SaveChanges();
        
        // assert
        Assert.Empty(context.Scripts.First(s => s.Id == testScriptTwo.Id).Includes);
        Assert.Empty(context.Scripts.First(s => s.Id == testScript.Id).IncludedIn);
    }

    #endregion

    #region GetAdventureScriptsWithSourceReference

    [Fact]
    public async Task GetAdventureScriptsWithSourceReference_ContainsReference_ReturnScript()
    {
        // arrange
        await using var context = new DatabaseContext(DbContextOptions);
        var testScript = new Script()
        {
            Adventure = new Adventure(),
            Content = Guid.NewGuid().ToString()
        };
        await context.Scripts.AddAsync(testScript);
        await context.SaveChangesAsync();
        var repository = new ScriptsRepository(context);
        
        // act
        var scripts = await repository.GetAdventureScriptsWithSourceReference(
            testScript.AdventureId, Guid.Parse(testScript.Content));
        
        // assert
        Assert.Single(scripts);
        Assert.Equal(testScript.Content, scripts[0].Content);
    }
    
    [Fact]
    public async Task GetAdventureScriptsWithSourceReference_NoContainsReference_ReturnEmpty()
    {
        // arrange
        await using var context = new DatabaseContext(DbContextOptions);
        var testScript = new Script()
        {
            Adventure = new Adventure(),
            Content = Guid.NewGuid().ToString()
        };
        await context.Scripts.AddAsync(testScript);
        await context.SaveChangesAsync();
        var repository = new ScriptsRepository(context);
        
        // act
        var scripts = await repository.GetAdventureScriptsWithSourceReference(
            testScript.AdventureId, Guid.NewGuid());
        
        // assert
        Assert.Empty(scripts);
    }

    #endregion
}