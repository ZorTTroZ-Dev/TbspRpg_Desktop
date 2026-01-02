using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgDataLayer.Tests.Repositories;

public class ScriptIncludesRepositoryTests() : InMemoryTest("ScriptIncludesRepositoryTests")
{
    #region AddScriptInclude

    [Fact]
    public async Task AddScriptInclude_IncludeAdded()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptIncluded = new Script()
        {
            Name = "test include script"
        };
        var testScriptInclude = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncluded,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        var repository = new ScriptIncludesRepository(context);
        
        // act
        await repository.AddScriptInclude(testScriptInclude);
        await repository.SaveChanges();
        
        // assert
        Assert.Single(context.ScriptIncludes);
    }
    
    #endregion
    
    #region GetScriptInclude
    
    [Fact]
    public async Task GetScriptInclude_IncludeExists_IncludeReturned()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptIncluded = new Script()
        {
            Name = "test include script"
        };
        var testScriptInclude = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncluded,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.ScriptIncludes.AddAsync(testScriptInclude);
        await context.SaveChangesAsync();
        var repository = new ScriptIncludesRepository(context);
        
        // act
        var include = await repository.GetScriptInclude(testScript.Id, testScriptIncluded.Id);
        
        // assert
        Assert.NotNull(include);
    }
    
    [Fact]
    public async Task GetScriptInclude_IncludeDoesntExist_ReturnNull()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptIncluded = new Script()
        {
            Name = "test include script"
        };
        var testScriptInclude = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncluded,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.ScriptIncludes.AddAsync(testScriptInclude);
        await context.SaveChangesAsync();
        var repository = new ScriptIncludesRepository(context);
        
        // act
        var include = await repository.GetScriptInclude(42, 43);
        
        // assert
        Assert.Null(include);
    }
    
    #endregion
    
    #region GetIncludes

    [Fact]
    public async Task GetIncludesForScriptId_NoIncludes_ReturnEmptyList()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptTwo = new Script()
        {
            Name = "test script Two"
        };
        var testScriptIncluded = new Script()
        {
            Name = "test include script"
        };
        var testScriptIncludedTwo = new Script()
        {
            Name = "test include script two"
        };
        var testScriptInclude = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncluded,
            Order = 1
        };
        var testScriptIncludeTwo = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncludedTwo,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Scripts.AddAsync(testScriptTwo);
        await context.ScriptIncludes.AddAsync(testScriptInclude);
        await context.ScriptIncludes.AddAsync(testScriptIncludeTwo);
        await context.SaveChangesAsync();
        var repository = new ScriptIncludesRepository(context);
        
        // act
        var includes = await repository.GetIncludesForScriptId(testScriptTwo.Id);

        // assert
        Assert.Empty(includes);
    }
    
    [Fact]
    public async Task GetIncludesForScriptId_MultipleIncludes_IncludesReturnedInOrder()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptIncluded = new Script()
        {
            Name = "test include script"
        };
        var testScriptIncludedTwo = new Script()
        {
            Name = "test include script two"
        };
        var testScriptInclude = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncluded,
            Order = 1
        };
        var testScriptIncludeTwo = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncludedTwo,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.ScriptIncludes.AddAsync(testScriptInclude);
        await context.ScriptIncludes.AddAsync(testScriptIncludeTwo);
        await context.SaveChangesAsync();
        var repository = new ScriptIncludesRepository(context);
        
        // act
        var includes = await repository.GetIncludesForScriptId(testScript.Id);

        // assert
        Assert.Equal(2, includes.Count);
        Assert.Equal("test include script two", includes[0].Includes.Name);
        Assert.Equal(testScriptIncludedTwo.Id, includes[0].IncludesId);
        Assert.Equal("test include script", includes[1].Includes.Name);
        Assert.Equal(testScriptIncluded.Id, includes[1].IncludesId);
        Assert.Equal(testScript.Id, includes[0].IncludedInId);
    }
    
    #endregion
    
    #region GetIncludedIn
    
    [Fact]
    public async Task GetIncludedInForScriptId_NotIncludedIn_ReturnEmptyList()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptTwo = new Script()
        {
            Name = "test script two"
        };
        var testScriptIncluded = new Script()
        {
            Name = "test include script"
        };
        var testScriptIncludedTwo = new Script()
        {
            Name = "test include script two"
        };
        var testScriptInclude = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncluded,
            Order = 0
        };
        var testScriptIncludeTwo = new ScriptInclude()
        {
            IncludedIn = testScriptTwo,
            Includes = testScriptIncluded,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.Scripts.AddAsync(testScriptIncludedTwo);
        await context.ScriptIncludes.AddAsync(testScriptInclude);
        await context.ScriptIncludes.AddAsync(testScriptIncludeTwo);
        await context.SaveChangesAsync();
        var repository = new ScriptIncludesRepository(context);
        
        // act
        var includes = await repository.GetIncludedInForScriptId(testScriptIncludedTwo.Id);

        // assert
        Assert.Empty(includes);
    }
    
    [Fact]
    public async Task GetIncludedInForScriptId_IncludedInMultiple_ListReturned()
    {
        // arrange
        var testScript = new Script()
        {
            Name = "test script"
        };
        var testScriptTwo = new Script()
        {
            Name = "test script two"
        };
        var testScriptIncluded = new Script()
        {
            Name = "test include script"
        };
        var testScriptInclude = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncluded,
            Order = 0
        };
        var testScriptIncludeTwo = new ScriptInclude()
        {
            IncludedIn = testScriptTwo,
            Includes = testScriptIncluded,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.ScriptIncludes.AddAsync(testScriptInclude);
        await context.ScriptIncludes.AddAsync(testScriptIncludeTwo);
        await context.SaveChangesAsync();
        var repository = new ScriptIncludesRepository(context);
        
        // act
        var includes = await repository.GetIncludedInForScriptId(testScriptIncluded.Id);

        // assert
        Assert.Equal(2, includes.Count);
        Assert.Equal("test include script", includes[0].Includes.Name);
        Assert.Equal("test include script", includes[1].Includes.Name);
        Assert.Equal(testScript.Id, includes[0].IncludedInId);
        Assert.Equal(testScriptTwo.Id, includes[1].IncludedInId);
    }
    
    #endregion
    
    #region RemoveIncludes
    
    [Fact]
    public async Task RemoveAllIncludes_IncludesRemoved()
    {
        // arrange
        var rootScript = new Script()
        {
            Name = "root"
        };
        var testScript = new Script()
        {
            Name = "test script"
        };
        var rootScriptInclude = new ScriptInclude()
        {
            IncludedIn = rootScript,
            Includes = testScript,
            Order = 0
        };
        var testScriptIncluded = new Script()
        {
            Name = "test include script"
        };
        var testScriptIncludedTwo = new Script()
        {
            Name = "test include script two"
        };
        var testScriptInclude = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncluded,
            Order = 1
        };
        var testScriptIncludeTwo = new ScriptInclude()
        {
            IncludedIn = testScript,
            Includes = testScriptIncludedTwo,
            Order = 0
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.ScriptIncludes.AddAsync(testScriptInclude);
        await context.ScriptIncludes.AddAsync(testScriptIncludeTwo);
        await context.ScriptIncludes.AddAsync(rootScriptInclude);
        await context.SaveChangesAsync();
        var repository = new ScriptIncludesRepository(context);
        
        // act
        repository.RemoveScriptIncludes(testScript.Id);
        await repository.SaveChanges();

        // assert
        Assert.Empty(context.ScriptIncludes);
    }

    #endregion
    
}