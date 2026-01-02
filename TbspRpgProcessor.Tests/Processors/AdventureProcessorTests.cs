using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor.Entities;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgProcessor.Tests.Processors
{
    public class AdventureProcessorTests: ProcessorTest
    {
        #region UpdateAdventure

        [Fact]
        public async Task UpdateAdventure_InvalidAdventureId_ExceptionThrown()
        {
            // arrange
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "test_adventure",
                InitialCopyKey = Guid.NewGuid()
            };
            var testSource = new En()
            {
                Id = 1,
                Key = testAdventure.InitialCopyKey,
                Name = "test_adventure",
                Text = "test source"
            };
            var adventures = new List<Adventure>() { testAdventure };
            var sources = new List<En>() {testSource};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = adventures,
                Sources = sources
            });
            
            // act
            Task Act() => processor.UpdateAdventure(new AdventureUpdateModel()
            {
                Adventure = new Adventure()
                {
                    Id = 2,
                    Name = "updated_test_adventure"
                },
                InitialSource = new En()
                {
                    Id = testSource.Id,
                    Key = testSource.Key,
                    Name = testSource.Name,
                    Text = testSource.Text
                },
                UserId = Guid.NewGuid(),
                Language = Languages.ENGLISH
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task UpdateAdventure_EmptyAdventureId_AdventureCreated()
        {
            // arrange
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "test_adventure",
                InitialCopyKey = Guid.NewGuid(),
                DescriptionCopyKey = Guid.NewGuid()
            };
            var testSource = new En()
            {
                Id = 1,
                Key = testAdventure.InitialCopyKey,
                Name = "test_adventure",
                Text = "test source",
                AdventureId = testAdventure.Id
            };
            var testDescriptionSource = new En()
            {
                Id = 2,
                Key = testAdventure.DescriptionCopyKey,
                Name = "description_test_adventure",
                Text = "test description source",
                AdventureId = testAdventure.Id
            };
            var adventures = new List<Adventure>() { testAdventure };
            var sources = new List<En>() {testSource, testDescriptionSource};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = adventures,
                Sources = sources
            });
            
            // act
            await processor.UpdateAdventure(new AdventureUpdateModel()
            {
                Adventure = new Adventure()
                {
                    Name = "new_test_adventure",
                    InitialCopyKey = Guid.Empty
                },
                InitialSource = new En()
                {
                    Key = Guid.Empty,
                    Text = "new_test source"
                },
                DescriptionSource = new En()
                {
                    Key = Guid.Empty,
                    Text = "new_test description source"
                },
                Language = Languages.ENGLISH
            });

            // assert
            Assert.Equal(2, adventures.Count);
            var newAdventure = adventures.FirstOrDefault(adv => adv.Name == "new_test_adventure");
            Assert.NotNull(newAdventure);
            Assert.Equal(4, sources.Count);
        }

        [Fact]
        public async Task UpdateAdventure_ExistingAdventure_AdventureUpdated()
        {
            // arrange
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "test_adventure",
                InitialCopyKey = Guid.NewGuid(),
                DescriptionCopyKey = Guid.NewGuid()
            };
            var testDescriptionSource = new En()
            {
                Id = 1,
                Key = testAdventure.DescriptionCopyKey,
                Name = "description_test_adventure",
                Text = "test description source",
                AdventureId = testAdventure.Id
            };
            var testSource = new En()
            {
                Id = 2,
                Key = testAdventure.InitialCopyKey,
                Name = "test_adventure",
                Text = "test source",
                AdventureId = testAdventure.Id
            };
            var adventures = new List<Adventure>() { testAdventure };
            var sources = new List<En>() {testSource, testDescriptionSource};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = adventures,
                Sources = sources
            });
            
            // act
            await processor.UpdateAdventure(new AdventureUpdateModel()
            {
                Adventure = new Adventure()
                {
                    Id = testAdventure.Id,
                    Name = "updated_adventure_name",
                    InitialCopyKey = testAdventure.InitialCopyKey,
                    DescriptionCopyKey = testAdventure.DescriptionCopyKey
                },
                InitialSource = new En()
                {
                    Id = testSource.Id,
                    Key = testSource.Key,
                    Name = testSource.Name,
                    Text = "updated source"
                },
                DescriptionSource = new En()
                {
                    Id = testDescriptionSource.Id,
                    Key = testDescriptionSource.Key,
                    Name = testDescriptionSource.Name,
                    Text = "updated description source"
                },
                Language = Languages.ENGLISH
            });

            // assert
            Assert.Single(adventures);
            Assert.Equal("updated_adventure_name", adventures[0].Name);
            Assert.Equal(2, sources.Count);
            Assert.Equal("updated source", sources.
                First(source => source.Id == testSource.Id).Text);
            Assert.Equal("updated description source", sources.
                First(source => source.Id == testDescriptionSource.Id).Text);
        }

        #endregion

        #region RemoveAdventure

        [Fact]
        public async Task RemoveAdventure_BadAdventureId_ExceptionThrown()
        {
            // arrange
            var testAdventureId = 1;
            var testSources = new List<En>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventureId,
                    Key = Guid.NewGuid(),
                    Text = "source one"
                },
                new()
                {
                    Id = 2,
                    AdventureId = testAdventureId,
                    Key = Guid.NewGuid(),
                    Text = "source two"
                }
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventureId
                },
                new()
                {
                    Id = 2,
                    AdventureId = testAdventureId
                }
            };
            var testContents = new List<Content>()
            {
                new()
                {
                    Id = 1,
                    GameId = testGames[0].Id,
                    Position = 0,
                    SourceKey = Guid.NewGuid()
                },
                new()
                {
                    Id = 2,
                    GameId = testGames[0].Id,
                    Position = 1,
                    SourceKey = Guid.NewGuid()
                },
                new()
                {
                    Id = 3,
                    GameId = testGames[1].Id,
                    Position = 0,
                    SourceKey = Guid.NewGuid()
                },
                new()
                {
                    Id = 4,
                    GameId = testGames[1].Id,
                    Position = 1,
                    SourceKey = Guid.NewGuid()
                }
            };
            var locationId = 1;
            var locationIdTwo = 2;
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = testAdventureId,
                    Name = "test adventure",
                    Games = testGames,
                    Locations = new List<Location>()
                    {
                        new()
                        {
                            Id = locationId,
                            Name = "test location",
                            Initial = true,
                            SourceKey = Guid.NewGuid(),
                            Routes = new List<Route>()
                            {
                                new()
                                {
                                    Id = 1,
                                    Name = "test route",
                                    LocationId = locationId
                                }
                            }
                        },
                        new()
                        {
                            Id = locationIdTwo,
                            Name = "test location two",
                            Initial = true,
                            SourceKey = Guid.NewGuid(),
                            Routes = new List<Route>()
                            {
                                new()
                                {
                                    Id = 2,
                                    Name = "test route",
                                    LocationId = locationIdTwo
                                }
                            }
                        }
                    }
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = testAdventures,
                Locations = testAdventures[0].Locations,
                Sources = testSources,
                Games = testGames,
                Contents = testContents
            });
            
            // act
            Task Act() => processor.RemoveAdventure(new AdventureRemoveModel()
            {
                AdventureId = 14
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task RemoveAdventure_Valid_AdventureRemoved()
        {
            // arrange
            var testAdventureId = 1;
            var testSources = new List<En>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventureId,
                    Key = Guid.NewGuid(),
                    Text = "source one"
                },
                new()
                {
                    Id = 2,
                    AdventureId = testAdventureId,
                    Key = Guid.NewGuid(),
                    Text = "source two"
                }
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventureId
                },
                new()
                {
                    Id = 2,
                    AdventureId = testAdventureId
                }
            };
            var testContents = new List<Content>()
            {
                new()
                {
                    Id = 1,
                    GameId = testGames[0].Id,
                    Position = 0,
                    SourceKey = Guid.NewGuid()
                },
                new()
                {
                    Id = 2,
                    GameId = testGames[0].Id,
                    Position = 1,
                    SourceKey = Guid.NewGuid()
                },
                new()
                {
                    Id = 3,
                    GameId = testGames[1].Id,
                    Position = 0,
                    SourceKey = Guid.NewGuid()
                },
                new()
                {
                    Id = 4,
                    GameId = testGames[1].Id,
                    Position = 1,
                    SourceKey = Guid.NewGuid()
                }
            };
            var locationId = 1;
            var locationIdTwo = 2;
            var testRoutes = new List<Route>
            {
                new()
                {
                    Id = 1,
                    Name = "test route",
                    LocationId = locationId
                },
                new()
                {
                    Id = 2,
                    Name = "test route",
                    LocationId = locationIdTwo
                }
            };
            var testLocations = new List<Location>()
            {
                new()
                {
                    Id = locationId,
                    Name = "test location",
                    Initial = true,
                    SourceKey = Guid.NewGuid(),
                    Routes = new List<Route>()
                    {
                        testRoutes[0]
                    }
                },
                new()
                {
                    Id = locationIdTwo,
                    Name = "test location two",
                    Initial = false,
                    SourceKey = Guid.NewGuid(),
                    Routes = new List<Route>()
                    {
                        testRoutes[1]
                    }
                }
            };
            var testScripts = new List<Script>()
            {
                new()
                {
                    Id = 1,
                    Name = "test script",
                    AdventureId = testAdventureId
                },
                new Script()
                {
                    Id = 1,
                    Name = "test script two",
                    AdventureId = testAdventureId
                }
            };
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = testAdventureId,
                    Name = "test adventure",
                    Games = new List<Game>() { testGames[0], testGames[1] },
                    Locations = new List<Location>() { testLocations[0], testLocations[1] },
                    InitializationScript = testScripts[0],
                    InitializationScriptId = testScripts[0].Id,
                    TerminationScript = testScripts[1],
                    TerminationScriptId = testScripts[1].Id
                }
            };
            
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = testAdventures,
                Locations = testLocations,
                Sources = testSources,
                Games = testGames,
                Contents = testContents,
                Scripts = testScripts,
                Routes = testRoutes
            });
            
            // act
            await processor.RemoveAdventure(new AdventureRemoveModel()
            {
                AdventureId = testAdventureId
            });
            
            // assert
            Assert.Empty(testAdventures);
            Assert.Empty(testLocations);
            Assert.Empty(testRoutes);
            Assert.Empty(testSources);
            Assert.Empty(testContents);
        }

        #endregion

        #region CreateAdventureInitial

        [Fact]
        public async Task CreateAdventureInitial_EmptyName_ExceptionThrown()
        {
            // arrange
            var adventures = new List<Adventure>();
            var sources = new List<En>();
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = adventures,
                Sources = sources
            });
            
            // act
            Task Act() => processor.CreateAdventureInitial(new AdventureCreateModel()
            {
                Name = "",
                Description = "description",
                //Language = Languages.ENGLISH
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }
        
        [Fact]
        public async Task CreateAdventureInitial_Valid_AdventureAndCopyCreated()
        {
            // arrange
            var adventures = new List<Adventure>();
            var copy = new List<Copy>();
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = adventures,
                Copy = copy
            });
            var languages = new List<Language>
            {
                new()
                {
                    Id = 1,
                    Code = "en",
                    Name = "English"
                },
                new()
                {
                    Id = 2,
                    Code = "es",
                    Name = "Spanish"
                }
            };
            
            // act
            await processor.CreateAdventureInitial(new AdventureCreateModel()
            {
                Name = "new adventure",
                Description = "description",
                Languages = languages,
                DescriptionLanguage = languages[0]
            });

            // assert
            Assert.Single(adventures);
            Assert.Equal(2, copy.Count);
            Assert.Equal("new adventure", adventures[0].Name);
            Assert.NotEqual(Guid.Empty, adventures[0].DescriptionCopyKey);
            Assert.Equal("description", copy.First(cpy => cpy.Language.Id == languages[0].Id).Text);
        }
        
        [Fact]
        public async Task CreateAdventureInitial_EmptyDescription_AdventureCreated()
        {
            // arrange
            var adventures = new List<Adventure>();
            var copy = new List<Copy>();
            var languages = new List<Language>
            {
                new()
                {
                    Id = 1,
                    Code = "en",
                    Name = "English"
                },
                new()
                {
                    Id = 2,
                    Code = "es",
                    Name = "Spanish"
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = adventures,
                Copy = copy
            });
            
            // act
            await processor.CreateAdventureInitial(new AdventureCreateModel()
            {
                Name = "new adventure",
                Description = "",
                Languages = languages,
                DescriptionLanguage = languages[0]
            });

            // assert
            Assert.Single(adventures);
            Assert.Equal("new adventure", adventures[0].Name);
            Assert.Equal(Guid.Empty, adventures[0].DescriptionCopyKey);
            Assert.Empty(copy);
        }

        #endregion
    }
}