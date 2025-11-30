using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor.Entities;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgProcessor.Tests.Processors;

public class AdventureObjectProcessorTests: ProcessorTest
{
    #region RemoveAdventure

    [Fact]
    public async Task RemoveAdventureObject_BadAdventureObjectId_ExceptionThrown()
    {
        // arrange
        var testAdventures = new List<Adventure>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure",
            }
        };
        var testAdventureObjects = new List<AdventureObject>()
        {
            new()
            {
                Id = 1,
                AdventureId = testAdventures[0].Id
            }
        };
        
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Adventures = testAdventures,
            AdventureObjects = testAdventureObjects
        });
        
        // act
        Task Act() => processor.RemoveAdventureObject(new AdventureObjectRemoveModel()
        {
            AdventureObjectId = 17
        });

        // assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }

    [Fact]
    public async Task RemoveAdventureObject_Valid_AdventureObjectRemoved()
    {
        // arrange
        var testAdventures = new List<Adventure>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure",
            }
        };
        var testAdventureObjects = new List<AdventureObject>()
        {
            new AdventureObject()
            {
                Id = 1,
                AdventureId = testAdventures[0].Id
            }
        };
            
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Adventures = testAdventures,
            AdventureObjects = testAdventureObjects
        });
        
        // act
        await processor.RemoveAdventureObject(new AdventureObjectRemoveModel()
        {
            AdventureObjectId = testAdventureObjects[0].Id
        });
        
        // assert
        Assert.Empty(testAdventureObjects);
    }

    #endregion

    #region UpdateAdventureObject
    
    [Fact]
    public async Task UpdateAdventureObject_EmptyId_CreateAdventureObject()
    {
        // arrange
        var testAdventures = new List<Adventure>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure",
            }
        };
        var testNameSource = new En()
        {
            Key = Guid.Empty,
            Name = "test_source",
            AdventureId = testAdventures[0].Id,
            Text = "content"
        };
        var testDescSource = new En()
        {
            Key = Guid.Empty,
            Name = "test_source",
            AdventureId = testAdventures[0].Id,
            Text = "content"
        };
        var testLocations = new List<Location>()
        {
            new()
            {
                Id = 1,
                AdventureId = testAdventures[0].Id
            }
        };
        var testAdventureObjects = new List<AdventureObject>();
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Adventures = testAdventures,
            Locations = testLocations,
            Sources = new List<En>() {testDescSource, testNameSource},
            AdventureObjects = testAdventureObjects
        });
        
        // act
        await processor.UpdateAdventureObject(new AdventureObjectUpdateModel()
        {
            AdventureObject = new AdventureObject()
            {
                AdventureId = testAdventures[0].Id,
                Name = "test adventure object",
                Description = "test adventure object",
                Locations = new List<Location>()
                {
                    testLocations[0]
                }
            },
            NameSource = testNameSource,
            DescriptionSource = testDescSource,
            Language = Languages.ENGLISH
        });
        
        // assert
        Assert.Single(testAdventureObjects);
        Assert.Single(testAdventureObjects[0].Locations);
        Assert.Equal("test adventure object", testAdventureObjects[0].Name);
    }
    
    [Fact]
    public async Task UpdateAdventureObject_InvalidId_ExceptionThrown()
    {
        // arrange
        var testAdventures = new List<Adventure>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure",
            }
        };
        var testAdventureObjects = new List<AdventureObject>()
        {
            new()
            {
                Id = 1,
                AdventureId = testAdventures[0].Id
            }
        };
            
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Adventures = testAdventures,
            AdventureObjects = testAdventureObjects
        });
        
        // act
        Task Act() => processor.UpdateAdventureObject(new AdventureObjectUpdateModel()
        {
            AdventureObject = new AdventureObject()
            {
                Id = 4,
                Name = "banana"
            }
        });
    
        // assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }
    
    [Fact]
    public async Task UpdateAdventureObject_ValidId_AdventureObjectUpdated()
    {
        // arrange
        var testAdventures = new List<Adventure>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure",
            }
        };
        var testSource = new En()
        {
            Id = 1,
            Key = Guid.NewGuid(),
            Name = "test_source",
            AdventureId = testAdventures[0].Id,
            Text = "content"
        };
        var testAdventureObjects = new List<AdventureObject>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure object",
                AdventureId = testAdventures[0].Id,
                NameSourceKey = testSource.Key,
                DescriptionSourceKey = testSource.Key
            }
        };
            
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Adventures = testAdventures,
            Sources = new List<En>() {testSource},
            AdventureObjects = testAdventureObjects
        });
        
        // act
        await processor.UpdateAdventureObject(new AdventureObjectUpdateModel()
        {
            AdventureObject = new AdventureObject()
            {
                Id = testAdventureObjects[0].Id,
                Name = "banana",
                AdventureId = testAdventures[0].Id,
                Type = AdventureObjectTypes.Generic,
                Locations = new List<Location>()
            },
            DescriptionSource = testSource,
            NameSource = testSource,
            Language = Languages.ENGLISH
        });
        
        // assert
        Assert.Single(testAdventureObjects);
        Assert.Equal("banana", testAdventureObjects[0].Name);
        Assert.Equal(AdventureObjectTypes.Generic, testAdventureObjects[0].Type);
    }
    
    [Fact]
    public async Task UpdateAdventureObject_ValidSourceId_SourceUpdated()
    {
        // arrange
        var testAdventures = new List<Adventure>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure",
            }
        };
        var testNameSource = new En()
        {
            Id = 1,
            Key = Guid.NewGuid(),
            Name = "test_source",
            AdventureId = testAdventures[0].Id,
            Text = "content"
        };
        var testDescSource = new En()
        {
            Id = 2,
            Key = Guid.NewGuid(),
            Name = "test_source",
            AdventureId = testAdventures[0].Id,
            Text = "content"
        };
        var testAdventureObjects = new List<AdventureObject>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure object",
                AdventureId = testAdventures[0].Id,
                NameSourceKey = testNameSource.Key,
                DescriptionSourceKey = testDescSource.Key
            }
        };
            
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Adventures = testAdventures,
            Sources = new List<En>() {testDescSource, testNameSource},
            AdventureObjects = testAdventureObjects
        });
        
        // act
        await processor.UpdateAdventureObject(new AdventureObjectUpdateModel()
        {
            AdventureObject = testAdventureObjects[0],
            DescriptionSource = new En()
            {
                Id = testDescSource.Id,
                Key = testDescSource.Key,
                Name = "test_source",
                Text = "desc content"
            },
            NameSource = new En()
            {
                Id = testNameSource.Id,
                Key = testNameSource.Key,
                Name = "test_source",
                Text = "name content"
            },
            Language = Languages.ENGLISH
        });
        
        // assert
        Assert.Single(testAdventureObjects);
        Assert.Equal(testNameSource.Key, testAdventureObjects[0].NameSourceKey);
        Assert.Equal("name content", testNameSource.Text);
        Assert.Equal(testDescSource.Key, testAdventureObjects[0].DescriptionSourceKey);
        Assert.Equal("desc content", testDescSource.Text);
    }
    
    [Fact]
    public async Task UpdateAdventureObject_EmptySourceKey_SourceCreated()
    {
        // arrange
        var testAdventures = new List<Adventure>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure",
            }
        };
        var testAdventureObjects = new List<AdventureObject>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure object",
                AdventureId = testAdventures[0].Id
            }
        };
        var testSources = new List<En>();
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Adventures = testAdventures,
            Sources = testSources,
            AdventureObjects = testAdventureObjects
        });
        
        // act
        await processor.UpdateAdventureObject(new AdventureObjectUpdateModel()
        {
            AdventureObject = testAdventureObjects[0],
            DescriptionSource = new En()
            {
                Key = Guid.Empty,
                Name = "test_source",
                Text = "desc content"
            },
            NameSource = new En()
            {
                Key = Guid.Empty,
                Name = "test_source",
                Text = "name content"
            },
            Language = Languages.ENGLISH
        });
        
        // assert
        Assert.Single(testAdventureObjects);
        Assert.Equal(2, testSources.Count);
        Assert.Equal(testSources[0].Key, testAdventureObjects[0].NameSourceKey);
        Assert.Equal("name content", testSources[0].Text);
        Assert.Equal(testSources[1].Key, testAdventureObjects[0].DescriptionSourceKey);
        Assert.Equal("desc content", testSources[1].Text);
    }
    
    [Fact]
    public async Task UpdateAdventureObject_InvalidSourceId_ExceptionThrown()
    {
        // arrange
        var testAdventures = new List<Adventure>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure",
            }
        };
        var testNameSource = new En()
        {
            Id = 1,
            Key = Guid.NewGuid(),
            Name = "test_source",
            AdventureId = testAdventures[0].Id,
            Text = "content"
        };
        var testDescSource = new En()
        {
            Id = 2,
            Key = Guid.NewGuid(),
            Name = "test_source",
            AdventureId = testAdventures[0].Id,
            Text = "content"
        };
        var testAdventureObjects = new List<AdventureObject>()
        {
            new()
            {
                Id = 1,
                Name = "test adventure object",
                AdventureId = testAdventures[0].Id,
                NameSourceKey = testNameSource.Key,
                DescriptionSourceKey = testDescSource.Key
            }
        };
            
        var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
        {
            Adventures = testAdventures,
            Sources = new List<En>() {testDescSource, testNameSource},
            AdventureObjects = testAdventureObjects
        });
        
        // act
        Task Act() => processor.UpdateAdventureObject(new AdventureObjectUpdateModel()
        {
            AdventureObject = testAdventureObjects[0],
            DescriptionSource = new En()
            {
                Id = 3,
                Key = Guid.NewGuid(),
                Name = "test_source",
                Text = "desc content"
            },
            NameSource = new En()
            {
                Id = 4,
                Key = Guid.NewGuid(),
                Name = "test_source",
                Text = "name content"
            },
            Language = Languages.ENGLISH
        });
        
        // assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }
    
    #endregion
}