using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;
using Xunit;

namespace TbspRpgDataLayer.Tests.Services
{
    public class RoutesServiceTests() : InMemoryTest("RoutesServiceTests")
    {
        private static IRoutesService CreateService(DatabaseContext context)
        {
            return new RoutesService(
                new RoutesRepository(context),
                NullLogger<RoutesService>.Instance);
        }

        #region GetRoutesForLocation

        [Fact]
        public async Task GetRoutesForLocation_Valid_ReturnRoutes()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocationId = Guid.NewGuid();
            var testLocation = new Location()
            {
                Name = "test location",
                Routes = new List<Route>()
                {
                    new()
                    {
                        Name = "route1"
                    },
                    new()
                    {
                        Name = "route2"
                    }
                }
            };
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var routes = await service.GetRoutesForLocation(testLocation.Id);
            
            // assert
            Assert.Equal(2, routes.Count);
            Assert.Equal(testLocation.Id, routes[0].LocationId);
        }
        
        [Fact]
        public async Task GetRoutesForLocation_InValidLocation_NoRoutes()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location",
                Routes = new List<Route>()
                {
                    new()
                    {
                        Name = "route1"
                    },
                    new()
                    {
                        Name = "route2"
                    }
                }
            };
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var routes = await service.GetRoutesForLocation(37);
            
            // assert
            Assert.Empty(routes);
        }

        #endregion
        
        #region GetRoutesForAdventure

        [Fact]
        public async Task GetRoutesForAdventure_ValidAdventure_RoutesReturned()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var locationOne = new Location()
            {
                Name = "test location",
                Adventure = new Adventure()
                {
                    Name = "test adventure"
                }
            };
            var locationTwo = new Location()
            {
                Name = "test location two",
                Adventure = new Adventure()
                {
                    Name = "test adventure two"
                }
            };
            var testRoute = new Route()
            {
                Name = "test",
                Location = locationOne
            };
            var testRouteTwo = new Route()
            {
                Name = "test_two",
                Location = locationTwo
            };
            await context.Locations.AddRangeAsync(locationOne, locationTwo);
            await context.Routes.AddRangeAsync(testRoute, testRouteTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var routes = await service.GetRoutesForAdventure(locationOne.AdventureId);
            
            //assert
            Assert.Single(routes);
            Assert.Equal(testRoute.Name, routes.First().Name);
        }

        [Fact]
        public async Task GetRoutesForAdventure_InvalidAdventure_NoRoutes()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var locationOne = new Location()
            {
                Name = "test location",
                Adventure = new Adventure()
                {
                    Name = "test adventure"
                }
            };
            var locationTwo = new Location()
            {
                Name = "test location two",
                Adventure = new Adventure()
                {
                    Name = "test adventure two"
                }
            };
            var testRoute = new Route()
            {
                Name = "test",
                Location = locationOne
            };
            var testRouteTwo = new Route()
            {
                Name = "test_two",
                Location = locationTwo
            };
            await context.Locations.AddRangeAsync(locationOne, locationTwo);
            await context.Routes.AddRangeAsync(testRoute, testRouteTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var routes = await service.GetRoutesForLocation(94);
            
            //assert
            Assert.Empty(routes);
        }

        #endregion

        #region GetRouteById

        [Fact]
        public async Task GetRouteById_Valid_ReturnRoute()
        {
            // arrange
            var testDestinationLocation = new Location();
            var testLocation = new Location();
            var testRoute = new Route()
            {
                Name = "test route",
                DestinationLocation = testDestinationLocation,
                Location = testLocation
            };
            await using var context = new DatabaseContext(DbContextOptions);
            context.Routes.Add(testRoute);
            context.Locations.Add(testDestinationLocation);
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var route = await service.GetRouteById(testRoute.Id);
            
            // assert
            Assert.NotNull(route);
            Assert.Equal(testRoute.Id, route.Id);
        }
        
        [Fact]
        public async Task GetRouteById_InValid_ReturnNull()
        {
            // arrange
            var testRoute = new Route()
            {
                Name = "test route"
            };
            await using var context = new DatabaseContext(DbContextOptions);
            context.Routes.Add(testRoute);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var route = await service.GetRouteById(58);
            
            // assert
            Assert.Null(route);
        }

        #endregion
        
        #region GetRoutes

        [Fact]
        public async Task GetRoutes_FilterByDestinationLocationId_ReturnsRoutes()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testroute = new Route()
            {
                Location = new Location(),
                DestinationLocation = new Location(),
                Name = "test route"
            };
            var testroute2 = new Route()
            {
                Location = new Location(),
                DestinationLocation = new Location(),
                Name = "test route two"
            };
            context.Routes.AddRange(testroute, testroute2);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            // act
            var routes = await service.GetRoutes(new RouteFilter()
            {
                DestinationLocationId = testroute.DestinationLocationId
            });
            
            // assert
            Assert.Single(routes);
            Assert.Equal("test route", routes[0].Name);
        }
        
        [Fact]
        public async Task GetRoutes_FilterByLocationId_ReturnsRoutes()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testroute = new Route()
            {
                Location = new Location(),
                Name = "test route"
            };
            var testroute2 = new Route()
            {
                Location = new Location(),
                Name = "test route two"
            };
            context.Routes.AddRange(testroute, testroute2);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            // act
            var routes = await service.GetRoutes(new RouteFilter()
            {
                LocationId = testroute.LocationId
            });
            
            // assert
            Assert.Single(routes);
            Assert.Equal("test route", routes[0].Name);
        }
        
        [Fact]
        public async Task GetRoutes_NoFilter_ReturnsAll()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testroute = new Route()
            {
                Location = new Location(),
                Name = "test route"
            };
            var testroute2 = new Route()
            {
                Location = new Location(),
                Name = "test route two"
            };
            context.Routes.AddRange(testroute, testroute2);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            // act
            var routes = await service.GetRoutes(null);
            
            // assert
            Assert.Equal(2, routes.Count);
        }

        #endregion

        #region RemoveRoute

        [Fact]
        public async Task RemoveRoute_RouteRemoved()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testroute = new Route()
            {
                Location = new Location(),
                DestinationLocation = new Location(),
                Name = "test route"
            };
            context.Routes.Add(testroute);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            // act
            var routeToRemove = context.Routes.First(route => route.Id == testroute.Id);
            service.RemoveRoute(routeToRemove);
            
            // assert
            await context.SaveChangesAsync();
            Assert.Empty(context.Routes);
        }

        #endregion
        
        #region AddRoute

        [Fact]
        public async Task AddRoute_RouteAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var service = CreateService(context);
            
            // act
            await service.AddRoute(new Route()
            {
                Name = "test route",
                Location = new Location(),
                SourceKey = Guid.NewGuid()
            });
            await service.SaveChanges();
            
            // assert
            Assert.Single(context.Routes);
            Assert.Equal("test route", context.Routes.First().Name);
        }

        #endregion

        #region RemoveScriptFromRoutes

        [Fact]
        public async Task RemoveScriptFromRoutes_ScriptRemoved()
        {
            // arrange
            var testRoute = new Route()
            {
                Name = "test route",
                RouteTakenScript = new Script()
                {
                    Name = "test script"
                }
            };
            var testRouteTwo = new Route()
            {
                Name = "test route two" 
            };
            await using var context = new DatabaseContext(DbContextOptions);
            await context.Routes.AddRangeAsync(testRoute, testRouteTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            await service.RemoveScriptFromRoutes(testRoute.RouteTakenScript.Id);
            await service.SaveChanges();
            
            // assert
            Assert.Null(context.Routes.First(route => route.Id == testRoute.Id).RouteTakenScript);
        }

        #endregion

        #region GetAdventureRoutesWithSource

        [Fact]
        public async Task GetAdventureRoutesWithSource_SourceKeyMatches_ReturnRoutes()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testRoute = new Route()
            {
                Location = new Location()
                {
                    Adventure = new Adventure()
                },
                SourceKey = Guid.NewGuid(),
                RouteTakenSourceKey = Guid.NewGuid()
            };
            await context.Routes.AddAsync(testRoute);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var routes = await service.GetAdventureRoutesWithSource(
                testRoute.Location.AdventureId, testRoute.SourceKey);
            
            // assert
            Assert.Single(routes);
            Assert.Equal(testRoute.SourceKey, routes[0].SourceKey);
        }

        [Fact]
        public async Task GetAdventureRoutesWithSource_RouteTakenKeyMatches_ReturnRoutes()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testRoute = new Route()
            {
                Location = new Location()
                {
                    Adventure = new Adventure()
                },
                SourceKey = Guid.NewGuid(),
                RouteTakenSourceKey = Guid.NewGuid()
            };
            await context.Routes.AddAsync(testRoute);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var routes = await service.GetAdventureRoutesWithSource(
                testRoute.Location.AdventureId, testRoute.RouteTakenSourceKey);
            
            // assert
            Assert.Single(routes);
            Assert.Equal(testRoute.RouteTakenSourceKey, routes[0].RouteTakenSourceKey);
        }

        [Fact]
        public async Task GetAdventureRoutesWithSource_NoMatch_ReturnEmptyList()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testRoute = new Route()
            {
                Location = new Location()
                {
                    Adventure = new Adventure()
                },
                SourceKey = Guid.NewGuid(),
                RouteTakenSourceKey = Guid.NewGuid()
            };
            await context.Routes.AddAsync(testRoute);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var routes = await service.GetAdventureRoutesWithSource(
                54, testRoute.SourceKey);
            
            // assert
            Assert.Empty(routes);
        }

        #endregion

        #region DoesAdventureRouteUseSource

        [Fact]
        public async Task DoesAdventureRouteUseSource_UsesSource_ReturnTrue()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testRoute = new Route()
            {
                Location = new Location()
                {
                    Adventure = new Adventure()
                },
                SourceKey = Guid.NewGuid(),
                RouteTakenSourceKey = Guid.NewGuid()
            };
            await context.Routes.AddAsync(testRoute);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var usesSource = await service.DoesAdventureRouteUseSource(
                testRoute.Location.AdventureId, testRoute.SourceKey);
            
            // assert
            Assert.True(usesSource);
        }

        [Fact]
        public async Task DoesAdventureRouteUseSource_NotUseSource_ReturnFalse()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testRoute = new Route()
            {
                Location = new Location()
                {
                    Adventure = new Adventure()
                },
                SourceKey = Guid.NewGuid(),
                RouteTakenSourceKey = Guid.NewGuid()
            };
            await context.Routes.AddAsync(testRoute);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var usesSource = await service.DoesAdventureRouteUseSource(
                testRoute.Location.AdventureId, Guid.NewGuid());
            
            // assert
            Assert.False(usesSource);
        }

        #endregion
    }
}