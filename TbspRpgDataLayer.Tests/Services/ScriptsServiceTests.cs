using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgDataLayer.Tests.Services;

public class ScriptsServiceTests() : InMemoryTest("ScriptsServiceTests")
{
    private static IScriptsService CreateService(DatabaseContext context)
    {
        return new ScriptsService(
            new ScriptsRepository(context),
            NullLogger<ScriptsService>.Instance);
    }
    
    #region GetScriptById

    [Fact]
    public async Task GetScriptById_InvalidId_ReturnNull()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test",
            Content = "print('banana');",
            Type = ScriptTypes.LuaScript
        };
        await using var context = new DatabaseContext(DbContextOptions);
        context.Scripts.Add(testScript);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        // act
        var script = await service.GetScriptById(676);

        // assert
        Assert.Null(script);
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
        var service = CreateService(context);
        
        // act
        var scripts = await service.GetScriptsForAdventure(testScripts[0].Adventure.Id);
        
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
        var service = CreateService(context);
        
        // act
        var scripts = await service.GetScriptsForAdventure(88);
        
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
        var service = CreateService(context);
        
        // act
        await service.AddScript(testScript);
        await service.SaveChanges();
        
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
        var service = CreateService(context);

        // act
        service.RemoveScript(testScript);
        await service.SaveChanges();
        
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
        var service = CreateService(context);
        
        // act
        service.RemoveScripts(new List<Script>() { testScript, testScriptTwo});
        await service.SaveChanges();
        
        // assert
        Assert.Empty(context.Scripts);
    }

    #endregion

    #region RemoveAllScriptsForAdventure

    [Fact]
    public async Task RemoveAllScriptsForAdventure_ScriptsRemoved()
    {
        // arrange
        var testAdventure = new Adventure()
        {
            Name = "test adventure"
        };
        var testScript = new Script()
        {
            Name = "test script",
            Adventure = testAdventure
        };
        var testScriptTwo = new Script()
        {
            Name = "test script two",
            Adventure = testAdventure
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Adventures.AddRangeAsync(testAdventure);
        await context.Scripts.AddRangeAsync(testScript, testScriptTwo);
        await context.SaveChangesAsync();
        var service = CreateService(context);
        
        // act
        await service.RemoveAllScriptsForAdventure(testAdventure.Id);
        await service.SaveChanges();
        
        // assert
        Assert.Empty(context.Scripts);
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
        var service = CreateService(context);
        
        // act
        var scripts = await service.GetAdventureScriptsWithSourceReference(
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
        var service = CreateService(context);
        
        // act
        var scripts = await service.GetAdventureScriptsWithSourceReference(
            testScript.AdventureId, Guid.NewGuid());
        
        // assert
        Assert.Empty(scripts);
    }

    #endregion

    #region IsSourceKeyReferenced

    [Fact]
    public async Task IsSourceKeyReferenced_Referenced_ReturnTrue()
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
        var service = CreateService(context);
        
        // act
        var referenced = await service.IsSourceKeyReferenced(testScript.AdventureId,
            Guid.Parse(testScript.Content));
        
        // assert
        Assert.True(referenced);
    }
    
    [Fact]
    public async Task IsSourceKeyReferenced_NotReferenced_ReturnFalse()
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
        var service = CreateService(context);
        
        // act
        var referenced = await service.IsSourceKeyReferenced(6745, 
            Guid.Parse(testScript.Content));
        
        // assert
        Assert.False(referenced);
    }

    #endregion
}