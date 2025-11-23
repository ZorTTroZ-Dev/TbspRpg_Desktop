using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using Xunit;

namespace TbspRpgDataLayer.Tests.Repositories
{
    public class RoutesRepositoryTests() : InMemoryTest("RoutesRepositoryTests")
    {
        #region GetRoutesForLocation

        [Fact]
        public async Task GetRoutesForLocation_ValidLocation_RoutesReturned()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location();
            var testRoute = new Route()
            {
                Name = "test",
                Location = testLocation
            };
            var testRouteTwo = new Route()
            {
                Name = "test_two",
                Location = testLocation
            };
            context.Routes.AddRange(testRoute, testRouteTwo);
            await context.SaveChangesAsync();
            var repo = new RoutesRepository(context);
            
            //act
            var routes = await repo.GetRoutesForLocation(testRoute.LocationId);
            
            //assert
            Assert.Equal(2, routes.Count);
            Assert.Equal(testRoute.Name, routes.First().Name);
        }

        [Fact]
        public async Task GetRoutesForLocation_InvalidLocation_NoRoutes()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testRoute = new Route()
            {
                Name = "test",
                Location = new Location()
            };
            context.Routes.Add(testRoute);
            await context.SaveChangesAsync();
            var repo = new RoutesRepository(context);
            
            //act
            var routes = await repo.GetRoutesForLocation(84);
            
            //assert
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
            var repo = new RoutesRepository(context);
            
            //act
            var routes = await repo.GetRoutesForAdventure(locationOne.AdventureId);
            
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
            var repo = new RoutesRepository(context);
            
            //act
            var routes = await repo.GetRoutesForLocation(65);
            
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
            var repository = new RoutesRepository(context);
            
            // act
            var route = await repository.GetRouteById(testRoute.Id);
            
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
            var repository = new RoutesRepository(context);
            
            // act
            var route = await repository.GetRouteById(68);
            
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
            var repository = new RoutesRepository(context);
            
            // act
            var routes = await repository.GetRoutes(new RouteFilter()
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
            var repository = new RoutesRepository(context);
            
            // act
            var routes = await repository.GetRoutes(new RouteFilter()
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
            var repository = new RoutesRepository(context);
            
            // act
            var routes = await repository.GetRoutes(null);
            
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
            var testroute2 = new Route()
            {
                Location = new Location(),
                DestinationLocation = new Location(),
                Name = "test route two"
            };
            context.Routes.AddRange(testroute, testroute2);
            await context.SaveChangesAsync();
            var repository = new RoutesRepository(context);
            
            // act
            var routeToRemove = context.Routes.First(route => route.Id == testroute.Id);
            repository.RemoveRoute(routeToRemove);
            
            // assert
            await context.SaveChangesAsync();
            Assert.Single(context.Routes);
            Assert.Equal("test route two", context.Routes.First().Name);
        }

        #endregion
        
        #region RemoveRoutes

        [Fact]
        public async Task RemoveRoutes_RoutesRemoved()
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
            var repository = new RoutesRepository(context);
            
            // act
            repository.RemoveRoutes(new List<Route>() { testroute, testroute2 });
            await context.SaveChangesAsync();
            
            // assert
            Assert.Empty(context.Routes);
        }

        #endregion

        #region AddRoute

        [Fact]
        public async Task AddRoute_RouteAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var repository = new RoutesRepository(context);
            
            // act
            await repository.AddRoute(new Route()
            {
                Name = "test route",
                Location = new Location(),
                SourceKey = Guid.NewGuid()
            });
            await repository.SaveChanges();
            
            // assert
            Assert.Single(context.Routes);
            Assert.Equal("test route", context.Routes.First().Name);
        }

        #endregion

        #region GetRoutesWithScript

        [Fact]
        public async Task GetRoutesWithScript_HasRoutes_RoutesReturned()
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
            var repository = new RoutesRepository(context);
            
            // act
            var routes = await repository.GetRoutesWithScript(testRoute.RouteTakenScript.Id);
            
            // assert
            Assert.Single(routes);
            Assert.Equal("test route", routes[0].Name);
        }

        [Fact]
        public async Task GetRoutesWithScript_NoRoutes_ReturnEmpty()
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
            var repository = new RoutesRepository(context);
            
            // act
            var routes = await repository.GetRoutesWithScript(71);
            
            // assert
            Assert.Empty(routes);
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
            var repository = new RoutesRepository(context);
            
            // act
            var routes = await repository.GetAdventureRoutesWithSource(
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
            var repository = new RoutesRepository(context);
            
            // act
            var routes = await repository.GetAdventureRoutesWithSource(
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
            var repository = new RoutesRepository(context);
            
            // act
            var routes = await repository.GetAdventureRoutesWithSource(
                75, testRoute.SourceKey);
            
            // assert
            Assert.Empty(routes);
        }

        #endregion
    }
}