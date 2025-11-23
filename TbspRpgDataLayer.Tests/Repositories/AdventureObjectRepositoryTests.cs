using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgDataLayer.Tests.Repositories;

public class AdventureObjectRepositoryTests() : InMemoryTest("AdventureObjectRepositoryTests")
{
    #region GetAdventureObjectById

    [Fact]
    public async Task GetAdventureObjectById_InvalidId_ReturnNull()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.SaveChangesAsync();
        var repository = new AdventureObjectRepository(context);

        // act
        var adventureObject = await repository.GetAdventureObjectById(17);

        // assert
        Assert.Null(adventureObject);
    }

    [Fact]
    public async Task GetAdventureObjectById_Valid_ReturnAdventureObject()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Locations = new List<Location>()
            {
                new Location()
                {
                    Name = "tl"
                },
                new Location()
                {
                    Name = "tl2"
                }
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.SaveChangesAsync();
        var repository = new AdventureObjectRepository(context);
        
        // act
        var adventureObject = await repository.GetAdventureObjectById(testObject.Id);
        
        // assert
        Assert.NotNull(adventureObject);
        Assert.Equal(testObject.Id, adventureObject.Id);
        Assert.Equal(2, adventureObject.Locations.Count);
    }
    
    #endregion
    
    #region GetAdventureObjectsForAdventure
    
    [Fact]
    public async Task GetAdventureObjectsForAdventure_ValidId_ReturnsAdventureObjects()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test"
            },
            Locations = new List<Location>()
            {
                new Location()
                {
                    Name = "tl"
                }    
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test two"
            },
            Locations = new List<Location>()
            {
                new Location()
                {
                    Name = "tl"
                }    
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var repository = new AdventureObjectRepository(context);
    
        // act
        var adventureObjects = 
            await repository.GetAdventureObjectsForAdventure(testObject.Adventure.Id);
        
        // assert
        Assert.Single(adventureObjects);
        Assert.Equal("test", adventureObjects[0].Adventure.Name);
        Assert.Single(adventureObjects[0].Locations);
    }
    
    [Fact]
    public async Task GetAdventureObjectsForAdventure_InvalidId_ReturnEmptyList()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test"
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test two"
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var repository = new AdventureObjectRepository(context);
        
        // act
        var adventureObjects = await repository.GetAdventureObjectsForAdventure(42);
        
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
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test"
            }
        };
        var repository = new AdventureObjectRepository(context);
        
        // act
        await repository.AddAdventureObject(testObject);
        await repository.SaveChanges();
        
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
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test"
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test two"
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var repository = new AdventureObjectRepository(context);
    
        // act
        repository.RemoveAdventureObject(testObject);
        await repository.SaveChanges();
        
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
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test"
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test two"
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var repository = new AdventureObjectRepository(context);
        
        // act
        repository.RemoveAdventureObjects(new List<AdventureObject>() { testObject, testObjectTwo });
        await repository.SaveChanges();
        
        // assert
        Assert.Empty(context.AdventureObjects);
    }
    
    #endregion
    
    #region GetAdventureObjectsByLocation
    
    [Fact]
    public async Task GetAdventureObjectsForLocation_ValidId_ReturnsAdventureObjects()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test"
            },
            Locations = new List<Location>()
            {
                new Location()
                {
                    Name = "testlocation"
                },
                new Location()
                {
                    Name = "testlocation2"
                }
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test two"
            },
            Locations = new List<Location>()
            {
                new Location()
                {
                    Name = "testlocation3"
                },
                new Location()
                {
                    Name = "testlocation4"
                }
            }
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var repository = new AdventureObjectRepository(context);
    
        // act
        var adventureObjects = 
            await repository.GetAdventureObjectsByLocation(testObject.Locations.First().Id);
        
        // assert
        Assert.Single(adventureObjects);
        Assert.Equal("test", adventureObjects[0].Adventure.Name);
        Assert.Equal(2, adventureObjects[0].Locations.Count);
    }
    
    [Fact]
    public async Task GetAdventureObjectsForLocation_ValidId_ReturnsMultipleAdventureObjects()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test"
            },
            Locations = new List<Location>()
            {
                new Location()
                {
                    Name = "testlocation"
                },
                new Location()
                {
                    Name = "testlocation2"
                }
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test two"
            },
            Locations = testObject.Locations
        };
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var repository = new AdventureObjectRepository(context);
    
        // act
        var adventureObjects = 
            await repository.GetAdventureObjectsByLocation(testObject.Locations.First().Id);
        
        // assert
        Assert.Equal(2, adventureObjects.Count);
        Assert.Equal(2, adventureObjects[0].Locations.Count);
        Assert.Equal(2, adventureObjects[1].Locations.Count);
    }
    
    [Fact]
    public async Task GetAdventureObjectsForLocation_InvalidId_ReturnEmptyList()
    {
        // arrange
        var testObject = new AdventureObject()
        {
            Name = "test object",
            Description = "test object",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test"
            },
            Locations = new List<Location>()
            {
                new Location()
                {
                    Name = "testlocation"
                },
                new Location()
                {
                    Name = "testlocation2"
                }
            }
        };
        var testObjectTwo = new AdventureObject()
        {
            Name = "test object two",
            Description = "test object two",
            Type = AdventureObjectTypes.Generic,
            Adventure = new Adventure()
            {
                Name = "test two"
            },
            Locations = new List<Location>()
            {
                new Location()
                {
                    Name = "testlocation3"
                },
                new Location()
                {
                    Name = "testlocation4"
                }
            }
        };
        
        await using var context = new DatabaseContext(DbContextOptions);
        await context.AdventureObjects.AddAsync(testObject);
        await context.AdventureObjects.AddAsync(testObjectTwo);
        await context.SaveChangesAsync();
        var repository = new AdventureObjectRepository(context);
        
        // act
        var adventureObjects = await repository.GetAdventureObjectsByLocation(42);
        
        // assert
        Assert.Empty(adventureObjects);
    }
    
    #endregion
}