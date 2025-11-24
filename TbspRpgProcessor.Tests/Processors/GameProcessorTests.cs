using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor.Entities;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgProcessor.Tests.Processors
{
    public class GameProcessorTests : ProcessorTest
    {
        #region StartGame
        
        [Fact]
        public async Task StartGame_InvalidAdventureId_ThrowsException()
        {
            // arrange
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = 1,
                    Name = "test adventure"
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = testAdventures
            });
            
            // act
            Task Act() => processor.StartGame(new GameStartModel()
            {
                AdventureId = 2,
                TimeStamp = DateTime.Now
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }
        
        [Fact]
        public async Task StartGame_GameExists_ReturnsGame()
        {
            // arrange
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = 1,
                    Name = "test adventure"
                }
            };
            var testGames = new List<Game>()
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
                Games = testGames
            });

            // act
            var game = await processor.StartGame(new GameStartModel()
            {
                AdventureId = testAdventures[0].Id,
                TimeStamp = DateTime.Now
            });

            // assert
            Assert.Single(testGames);
            Assert.NotNull(game);
            Assert.Equal(testAdventures[0].Id, game.AdventureId);
        }
        
        [Fact]
        public async Task StartGame_LocationDoesntExist_ThrowsException()
        {
            // arrange
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = 1,
                    Name = "test adventure"
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Adventures = testAdventures,
                Locations = new List<Location>(),
                Games = new List<Game>()
            });
            
            // act
            Task Act() => processor.StartGame(new GameStartModel()
            {
                AdventureId = testAdventures[0].Id,
                TimeStamp = DateTime.Now
            });

            // assert
            await Assert.ThrowsAsync<Exception>(Act);
        }
        
        [Fact]
        public async Task StartGame_ValidWithInitScript_GameCreatedStateUpdated()
        {
            // arrange
            var testScript = new Script()
            {
                Id = 1,
                Name = "test script",
                Content = @"
                    function run()
                        game:SetGameStatePropertyBoolean('GameInitialized', true)
		                result = true
	                end
                ",
                Type = ScriptTypes.LuaScript
            };
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = 1,
                    Name = "test adventure",
                    InitialSourceKey = Guid.NewGuid(),
                    InitializationScriptId = testScript.Id
                }
            };
            var testLocations = new List<Location>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventures[0].Id,
                    Initial = true,
                    SourceKey = Guid.NewGuid()
                }
            };
            var testContents = new List<Content>();
            var testGames = new List<Game>();
            var testScripts = new List<Script>() { testScript };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Scripts = testScripts,
                Adventures = testAdventures,
                Locations = testLocations,
                Games = testGames,
                Contents = testContents
            });
            
            // act
            var game = await processor.StartGame(new GameStartModel()
            {
                AdventureId = testAdventures[0].Id,
                TimeStamp = DateTime.UtcNow
            });

            // assert
            Assert.Single(testGames);
            Assert.NotNull(game);
            Assert.True(game.LocationUpdateTimeStamp > 0);
            Assert.Equal(testAdventures[0].Id, game.AdventureId);
            Assert.Equal(testLocations[0].Id, game.LocationId);
            Assert.Equal(2, testContents.Count);
            Assert.NotNull(testContents.FirstOrDefault(c => c.SourceKey == testAdventures[0].InitialSourceKey));
            Assert.NotNull(testContents.FirstOrDefault(c => c.SourceKey == testLocations[0].SourceKey));
            Assert.Equal("{\"GameInitialized\":true}", game.GameState);
        }
        
        [Fact]
        public async Task StartGame_ValidWithoutInitScript_GameCreated()
        {
            // arrange
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = 1,
                    Name = "test adventure",
                    InitialSourceKey = Guid.NewGuid(),
                    InitializationScriptId = null
                }
            };
            var testLocations = new List<Location>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventures[0].Id,
                    Initial = true,
                    SourceKey = Guid.NewGuid()
                }
            };
            var testContents = new List<Content>();
            var testGames = new List<Game>();
            var testScripts = new List<Script>();
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Scripts = testScripts,
                Adventures = testAdventures,
                Locations = testLocations,
                Games = testGames,
                Contents = testContents
            });
            
            // act
            var game = await processor.StartGame(new GameStartModel()
            {
                AdventureId = testAdventures[0].Id,
                TimeStamp = DateTime.UtcNow
            });

            // assert
            Assert.Single(testGames);
            Assert.NotNull(game);
            Assert.True(game.LocationUpdateTimeStamp > 0);
            Assert.Equal(testAdventures[0].Id, game.AdventureId);
            Assert.Equal(testLocations[0].Id, game.LocationId);
            Assert.Equal(2, testContents.Count);
            Assert.NotNull(testContents.FirstOrDefault(c => c.SourceKey == testAdventures[0].InitialSourceKey));
            Assert.NotNull(testContents.FirstOrDefault(c => c.SourceKey == testLocations[0].SourceKey));
        }

        #endregion

        #region RemoveGame

        [Fact]
        public async Task RemoveGame_InvalidGameId_ExceptionThrown()
        {
            // arrange
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = 1,
                    Name = "test adventure"
                }
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventures[0].Id
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Adventures = testAdventures,
                Games = testGames
            });
            
            // act
            Task Act() => processor.RemoveGame(new GameRemoveModel()
            {
                GameId = 2
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task RemoveGame_NoContent_GameRemoved()
        {
            // arrange
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = 1,
                    Name = "test adventure"
                }
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventures[0].Id
                }
            };
            var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
                Adventures = testAdventures,
                Games = testGames,
                Contents = new List<Content>()
            });
            
            // act
            await processor.RemoveGame(new GameRemoveModel()
            {
                GameId = testGames[0].Id
            });
            
            // assert
            Assert.Empty(testGames);
        }

        [Fact]
        public async Task RemoveGame_Valid_GameAndContentRemoved()
        {
            // arrange
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = 1,
                    Name = "test adventure"
                }
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventures[0].Id
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
                }
            };
            var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
                Adventures = testAdventures,
                Games = testGames,
                Contents = testContents
            });
            
            // act
            await processor.RemoveGame(new GameRemoveModel()
            {
                GameId = testGames[0].Id
            });
            
            // assert
            Assert.Empty(testGames);
            Assert.Empty(testContents);
        }

        #endregion

        #region RemoveGames

        [Fact]
        public async Task RemoveGames_AllGamesRemoved()
        {
            // arrange
            var testAdventures = new List<Adventure>()
            {
                new()
                {
                    Id = 1,
                    Name = "test adventure"
                }
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    AdventureId = testAdventures[0].Id
                },
                new()
                {
                    Id = 2,
                    AdventureId = testAdventures[0].Id
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
            var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
                Adventures = testAdventures,
                Games = testGames,
                Contents = testContents
            });
            
            // act
            await processor.RemoveGames(new GamesRemoveModel()
            {
                Games = new List<Game>()
                {
                    testGames[0], testGames[1]
                }
            });
            
            // assert
            Assert.Empty(testGames);
            Assert.Empty(testContents);
        }

        #endregion
    }
}