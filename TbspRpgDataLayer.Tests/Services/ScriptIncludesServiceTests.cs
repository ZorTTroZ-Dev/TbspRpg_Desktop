using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;
using Xunit;

namespace TbspRpgDataLayer.Tests.Services;

public class ScriptIncludesServiceTests() : InMemoryTest("ScriptIncludesServiceTests")
{
    private static IScriptIncludesService CreateService(DatabaseContext context)
    {
        return new ScriptIncludesService(
            new ScriptIncludesRepository(context),
            NullLogger<ScriptIncludesService>.Instance);
    }
    
    #region CollectAllIncludes

    [Fact]
    public async Task CollectAllIncludes_NoIncludes_EmptyList()
    {
        // arrange
        var script = new Script()
        {
            Name = "test_script"
        };
        var firstInclude = new Script()
        {
            Name = "first_include"
        };
        var secondInclude = new Script()
        {
            Name = "second_include"
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Scripts.AddRangeAsync(new List<Script>() { script, firstInclude, secondInclude });
        await context.SaveChangesAsync();
        var service = CreateService(context);
        
        // act
        var includes = await service.CollectAllIncludes(script.Id);
        
        // assert
        Assert.Empty(includes);
    }

    [Fact]
    public async Task CollectAllIncludes_OneIncludes_IncludeFound()
    {
        // arrange
        var script = new Script()
        {
            Name = "test_script"
        };
        var firstInclude = new Script()
        {
            Name = "first_include"
        };
        var secondInclude = new Script()
        {
            Name = "second_include"
        };
        var include = new ScriptInclude()
        {
            IncludedIn = script,
            Includes = firstInclude,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Scripts.AddRangeAsync(new List<Script>() { script, firstInclude, secondInclude });
        await context.ScriptIncludes.AddRangeAsync(new List<ScriptInclude>() { include });
        await context.SaveChangesAsync();
        var service = CreateService(context);
        
        // act
        var includes = await service.CollectAllIncludes(script.Id);
        
        // assert
        Assert.Single(includes);
    }
    
    [Fact]
    public async Task CollectAllIncludes_MultipleLevelIncludes_IncludesFound()
    {
        // arrange
        var script = new Script()
        {
            Name = "test_script"
        };
        var firstInclude = new Script()
        {
            Name = "first_include"
        };
        var secondInclude = new Script()
        {
            Name = "second_include"
        };
        var include = new ScriptInclude()
        {
            IncludedIn = script,
            Includes = firstInclude,
            Order = 0
        };
        var includeTwo = new ScriptInclude()
        {
            IncludedIn = firstInclude,
            Includes = secondInclude,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Scripts.AddRangeAsync(new List<Script>() { script, firstInclude, secondInclude });
        await context.ScriptIncludes.AddRangeAsync(new List<ScriptInclude>() { include, includeTwo });
        await context.SaveChangesAsync();
        var service = CreateService(context);
        
        // act
        var includes = await service.CollectAllIncludes(script.Id);
        
        // assert
        Assert.Equal(2, includes.Count);
    }
    
    [Fact]
    public async Task CollectAllIncludes_MultipleLevelCommonIncludes_IncludesFoundNoDuplicates()
    {
        // arrange
        var script = new Script()
        {
            Name = "test_script"
        };
        var firstInclude = new Script()
        {
            Name = "first_include"
        };
        var secondInclude = new Script()
        {
            Name = "second_include"
        };
        var commonInclude = new Script()
        {
            Name = "common_include"
        };
        var include = new ScriptInclude()
        {
            IncludedIn = script,
            Includes = firstInclude,
            Order = 0
        };
        var includeTwo = new ScriptInclude()
        {
            IncludedIn = firstInclude,
            Includes = secondInclude,
            Order = 0
        };
        var commonIncludeInclude = new ScriptInclude()
        {
            IncludedIn = script,
            Includes = commonInclude,
            Order = 1
        };
        var commonIncludeIncludeTwo = new ScriptInclude()
        {
            IncludedIn = firstInclude,
            Includes = commonInclude,
            Order = 1
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Scripts.AddRangeAsync(new List<Script>() { script, firstInclude, secondInclude, commonInclude });
        await context.ScriptIncludes.AddRangeAsync(new List<ScriptInclude>() { include, includeTwo, commonIncludeInclude, commonIncludeIncludeTwo });
        await context.SaveChangesAsync();
        var service = CreateService(context);
        
        // act
        var includes = await service.CollectAllIncludes(script.Id);
        
        // assert
        Assert.Equal(3, includes.Count);
    }
    
    [Fact]
    public async Task CollectAllIncludes_InvalidId_EmptyList()
    {
        // arrange
        var script = new Script()
        {
            Name = "test_script"
        };
        var firstInclude = new Script()
        {
            Name = "first_include"
        };
        var secondInclude = new Script()
        {
            Name = "second_include"
        };
        var include = new ScriptInclude()
        {
            IncludedIn = script,
            Includes = firstInclude,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Scripts.AddRangeAsync(new List<Script>() { script, firstInclude, secondInclude });
        await context.ScriptIncludes.AddRangeAsync(new List<ScriptInclude>() { include });
        await context.SaveChangesAsync();
        var service = CreateService(context);
        
        // act
        var includes = await service.CollectAllIncludes(42);
        
        // assert
        Assert.Empty(includes);
    }
    
    #endregion
}