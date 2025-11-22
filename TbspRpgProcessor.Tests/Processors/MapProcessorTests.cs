using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor.Entities;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgProcessor.Tests.Processors
{
    public class MapProcessorTests: ProcessorTest
    {
        #region ChangeLocationViaRoute

        [Fact]
        public async Task ChangeLocationViaRoute_InvalidGameId_ThrowsExcpetion()
        {
            // arrange
            var testRoute = new Route()
            {
                Id = 1,
                LocationId = 1,
                DestinationLocationId = 2,
                RouteTakenSourceKey = Guid.NewGuid()
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    LocationId = testRoute.LocationId
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Routes = new List<Route>() {testRoute},
                Games = testGames,
                Contents = new List<Content>()
            });
            
            // act
            Task Act() => processor.ChangeLocationViaRoute(new MapChangeLocationModel() {
                GameId = 2,
                RouteId = testRoute.Id,
                TimeStamp = DateTime.UtcNow
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }
        
        [Fact]
        public async Task ChangeLocationViaRoute_InvalidRouteId_ThrowsExcpetion()
        {
            // arrange
            var testRoute = new Route()
            {
                Id = 1,
                LocationId = 1,
                DestinationLocationId = 2,
                RouteTakenSourceKey = Guid.NewGuid()
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    LocationId = testRoute.LocationId
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Routes = new List<Route>() {testRoute},
                Games = testGames,
                Contents = new List<Content>()
            });
            
            // act
            Task Act() => processor.ChangeLocationViaRoute(new MapChangeLocationModel() {
                GameId = testGames[0].Id,
                RouteId = 8, 
                TimeStamp = DateTime.UtcNow
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }
        
        [Fact]
        public async Task ChangeLocationViaRoute_GameWrongLocation_ThrowsExcpetion()
        {
            // arrange
            var testRoute = new Route()
            {
                Id = 1,
                LocationId = 1,
                DestinationLocationId = 2,
                RouteTakenSourceKey = Guid.NewGuid()
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    LocationId = 17
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Routes = new List<Route>() {testRoute},
                Games = testGames,
                Contents = new List<Content>()
            });
            
            // act
            Task Act() => processor.ChangeLocationViaRoute(new MapChangeLocationModel() {
                GameId = testGames[0].Id,
                RouteId = testRoute.Id,
                TimeStamp = DateTime.UtcNow
            });

            // assert
            await Assert.ThrowsAsync<Exception>(Act);
        }
        
        [Fact]
        public async Task ChangeLocationViaRoute_Valid_LocationUpdated()
        {
            // arrange
            var testDestinationLocation = new Location()
            {
                Id = 1,
                SourceKey = Guid.NewGuid()
            };
            var testLocation = new Location()
            {
                Id = 2
            };
            var testRoute = new Route()
            {
                Id = 1,
                LocationId = testLocation.Id,
                Location = testLocation,
                DestinationLocationId = testDestinationLocation.Id,
                DestinationLocation = testDestinationLocation,
                RouteTakenSourceKey = Guid.NewGuid()
            };
            var testSources = new List<En>()
            {
                new()
                {
                    Id = 1,
                    Key = testRoute.RouteTakenSourceKey
                },
                new()
                {
                    Id = 2,
                    Key = testDestinationLocation.SourceKey
                }
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    LocationId = testRoute.LocationId
                }
            };
            var testContents = new List<Content>();
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Routes = new List<Route>() {testRoute},
                Sources = testSources,
                Games = testGames,
                Contents = testContents
            });
            
            // act
            await processor.ChangeLocationViaRoute(new MapChangeLocationModel() {
                GameId = testGames[0].Id,
                RouteId = testRoute.Id,
                TimeStamp = DateTime.UtcNow
            });
            
            // assert
            var game = testGames[0];
            Assert.Equal(testRoute.DestinationLocationId, game.LocationId);
            Assert.True(game.LocationUpdateTimeStamp > 0);
            Assert.Equal(2, testContents.Count);
            Assert.Equal(testContents[0].SourceKey, testRoute.RouteTakenSourceKey);
        }
        
        [Fact]
        public async Task ChangeLocationViaRoute_ValidFinalLocationWithScripts_LocationUpdated()
        {
            var exitLocationTestScript = new Script()
            {
                Id = 1,
                Name = "test script",
                Content = @"
                    function run()
                        game:SetGameStatePropertyBoolean('LocationExited', true)
		                result = true
	                end
                ",
                Type = ScriptTypes.LuaScript
            };
            var enterLocationTestScript = new Script()
            {
                Id = 2,
                Name = "test script",
                Content = @"
                    function run()
                        routeTaken = game:GetGameStatePropertyNumber('RouteTaken')
                        game:SetGameStatePropertyNumber('LocationEntered', routeTaken + 1)
                        
		                result = true
	                end
                ",
                Type = ScriptTypes.LuaScript
            };
            var routeTakenTestScript = new Script()
            {
                Id = 3,
                Name = "test script",
                Content = @"
                    function run()
                        game:SetGameStatePropertyNumber('RouteTaken', 42)
		                result = true
	                end
                ",
                Type = ScriptTypes.LuaScript
            };
            var terminationTestScript = new Script()
            {
                Id = 4,
                Name = "test script",
                Content = @"
                    function run()
                        game:SetGameStatePropertyBoolean('GameTerminated', true)
		                result = true
	                end
                ",
                Type = ScriptTypes.LuaScript
            };
            // arrange
            var testDestinationLocation = new Location()
            {
                Id = 1,
                SourceKey = Guid.NewGuid(),
                Final = true,
                EnterScriptId = enterLocationTestScript.Id
            };
            var testLocation = new Location()
            {
                Id = 2,
                ExitScriptId = exitLocationTestScript.Id
            };
            var testRoute = new Route()
            {
                Id = 1,
                LocationId = testLocation.Id,
                Location = testLocation,
                DestinationLocationId = testDestinationLocation.Id,
                DestinationLocation = testDestinationLocation,
                RouteTakenSourceKey = Guid.NewGuid(),
                RouteTakenScriptId = routeTakenTestScript.Id
            };
            var testSources = new List<En>()
            {
                new()
                {
                    Id = 1,
                    Key = testRoute.RouteTakenSourceKey
                },
                new()
                {
                    Id = 2,
                    Key = testDestinationLocation.SourceKey
                }
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    LocationId = testRoute.LocationId,
                    Adventure = new Adventure()
                    {
                        Id = 1,
                        Name = "test",
                        TerminationScriptId = terminationTestScript.Id
                    }
                }
            };
            var testContents = new List<Content>();
            var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
                Scripts = new List<Script>() { exitLocationTestScript, enterLocationTestScript, routeTakenTestScript, terminationTestScript },
                Routes = new List<Route>() {testRoute},
                Sources = testSources,
                Games = testGames,
                Contents = testContents
            });
            
            // act
            await processor.ChangeLocationViaRoute(new MapChangeLocationModel() {
                GameId = testGames[0].Id,
                RouteId = testRoute.Id,
                TimeStamp = DateTime.UtcNow
            });
            
            // assert
            var game = testGames[0];
            Assert.Equal(testRoute.DestinationLocationId, game.LocationId);
            Assert.True(game.LocationUpdateTimeStamp > 0);
            Assert.Equal(2, testContents.Count);
            Assert.Equal(testContents[0].SourceKey, testRoute.RouteTakenSourceKey);
            Assert.NotNull(game.GameState);
            Assert.Equal("{\"LocationExited\":true,\"RouteTaken\":42,\"LocationEntered\":43,\"GameTerminated\":true}", game.GameState);
        }
        
        [Fact]
        public async Task ChangeLocationViaRoute_ResolveDestinationSourceKey_LocationUpdated()
        {
            var testScript = new Script()
            {
                Id = 1,
                Name = "test script",
                Content = @"
                    function run()
                        game:SetGameStatePropertyBoolean('ScriptRun', true)
		                result = true
	                end
                ",
                Type = ScriptTypes.LuaScript
            };
            var resultSourceKey = Guid.NewGuid();
            var badResultSourceKey = Guid.NewGuid();
            
            // arrange
            var testDestinationLocation = new Location()
            {
                Id = 1,
                SourceKey = Guid.NewGuid(),
                Final = true,
                EnterScriptId = testScript.Id
            };
            var testLocation = new Location()
            {
                Id = 2,
                ExitScriptId = testScript.Id
            };
            var testRoute = new Route()
            {
                Id = 1,
                LocationId = testLocation.Id,
                Location = testLocation,
                DestinationLocationId = testDestinationLocation.Id,
                DestinationLocation = testDestinationLocation,
                RouteTakenSourceKey = Guid.NewGuid(),
                RouteTakenScriptId = testScript.Id
            };
            var testSources = new List<En>()
            {
                new()
                {
                    Id = 1,
                    Key = testRoute.RouteTakenSourceKey
                },
                new()
                {
                    Id = 2,
                    Key = resultSourceKey
                },
                new()
                {
                    Id = 3,
                    Key = badResultSourceKey
                },
                new()
                {
                    Id = 4,
                    Key = testDestinationLocation.SourceKey
                }
            };
            var testGames = new List<Game>()
            {
                new()
                {
                    Id = 1,
                    LocationId = testRoute.LocationId,
                    Adventure = new Adventure()
                    {
                        Id = 1,
                        Name = "test",
                        TerminationScriptId = testScript.Id
                    }
                }
            };
            var testContents = new List<Content>();
            var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
                Scripts = new List<Script>() { testScript },
                Routes = new List<Route>() {testRoute},
                Sources = testSources,
                Games = testGames,
                Contents = testContents
            });
            
            // act
            await processor.ChangeLocationViaRoute(new MapChangeLocationModel() {
                GameId = testGames[0].Id,
                RouteId = testRoute.Id,
                TimeStamp = DateTime.UtcNow
            });
            
            // assert
            var game = testGames[0];
            Assert.Equal(testRoute.DestinationLocationId, game.LocationId);
            Assert.True(game.LocationUpdateTimeStamp > 0);
            Assert.Equal(2, testContents.Count);
            Assert.Equal(testRoute.RouteTakenSourceKey, testContents[0].SourceKey);
            Assert.Equal(testDestinationLocation.SourceKey, testContents[1].SourceKey);
        }

        #endregion
    }
}