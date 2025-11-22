using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgDataLayer.Tests.Services;

public class AdventureObjectServiceTests() : InMemoryTest("AdventureObjectServiceTests")
{
    private static IAdventureObjectService CreateService(DatabaseContext context)
    {
        return new AdventureObjectService(
            new AdventureObjectRepository(context),
            NullLogger<AdventureObjectService>.Instance);
    }
    
    #region GetAdventureObjectById

    [Fact]
    public async Task GetAdventureObjectById_InvalidId_ReturnNull()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Id = 1,
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        // act
        var adventureObject = await service.GetAdventureObjectById(42);

        // assert
        Assert.Null(adventureObject);
    }

    [Fact]
    public async Task GetAdventureObjectById_Valid_ReturnAdventureObject()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Id = 1,
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        // act
        var adventureObject = await service.GetAdventureObjectById(testObject.Id);

        // assert
        Assert.NotNull(adventureObject);
        Assert.Equal(testObject.Id, adventureObject.Id);
    }

    #endregion

    #region GetAdventureObjectsForAdventure

    [Fact]
    public async Task GetAdventureObjectsForAdventure_ValidId_ReturnsAdventureObjects()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Id = 1,
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Id = 2,
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Id = 2,
                Name = "test two"
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        // act
        var adventureObjects = 
            await service.GetAdventureObjectsForAdventure(testObject.Adventure.Id);
        
        // assert
        Assert.Single(adventureObjects);
        Assert.Equal("test", adventureObjects[0].Adventure.Name);
    }

    [Fact]
    public async Task GetAdventureObjectsForAdventure_InvalidId_ReturnEmptyList()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Id = 1,
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Id = 2,
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Id = 2,
                Name = "test two"
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var service = CreateService(context);
        
        // act
        var adventureObjects = await service.GetAdventureObjectsForAdventure(42);
        
        // assert
        Assert.Empty(adventureObjects);
    }

    #endregion
    
    #region AddAdventureObject

    [Fact]
    public async Task AddAdventureObject_AdventureObjectAdded()
    {
        // arrange
        await using var context = new DatabaseContext(DbContextOptions);
        var testObject = new AdventureObject()
        {
            Id = 1,
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            }
        };
        var service = CreateService(context);
        
        // act
        await service.AddAdventureObject(testObject);
        await service.SaveChanges();
        
        // assert
        Assert.Single(context.AdventureObjects);
    }

    #endregion
        
    #region RemoveAdventureObject

    [Fact]
    public async Task RemoveAdventureObject_AdventureObjectRemoved()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Id = 1,
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Id = 2,
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Id = 2,
                Name = "test two"
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        // act
        service.RemoveAdventureObject(testObject);
        await service.SaveChanges();
        
        // assert
        Assert.Single(context.AdventureObjects);
    }
    
    #endregion

    #region RemoveAdventureObjects

    [Fact]
    public async Task RemoveAdventureObjects_AdventureObjectsRemoved()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Id = 1,
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Id = 2,
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Id = 2,
                Name = "test two"
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var service = CreateService(context);
        
        // act
        service.RemoveAdventureObjects(new List<AdventureObject>() { testObject, testObjectTwo });
        await service.SaveChanges();
        
        // assert
        Assert.Empty(context.AdventureObjects);
    }

    #endregion
}