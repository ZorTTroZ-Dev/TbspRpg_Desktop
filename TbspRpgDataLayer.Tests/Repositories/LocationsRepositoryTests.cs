using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using Xunit;

namespace TbspRpgDataLayer.Tests.Repositories
{
    public class LocationsRepositoryTests() : InMemoryTest("LocationsRepositoryTests")
    {
        #region GetInitialLocation

        [Fact]
        public async Task GetInitialLocation_Valid_ReturnLocation()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure
            {
                Locations = new List<Location>()
                {
                    new()
                    {
                        Initial = true
                    },
                    new()
                    {
                        Initial = false
                    }
                },
                Name = "TestOne"
            };
            context.Adventures.Add(testAdventure);
            await context.SaveChangesAsync();
            var locationRepository = new LocationsRepository(context);
            
            //act
            var location = await locationRepository.GetInitialForAdventure(testAdventure.Id);

            //assert
            Assert.Equal(testAdventure.Id, location.AdventureId);
            Assert.True(location.Initial);
        }
        
        [Fact]
        public async Task GetInitialLocation_Invalid_ReturnNothing()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure
            {
                Locations = new List<Location>()
                {
                    new()
                    {
                        Initial = true
                    },
                    new()
                    {
                        Initial = false
                    }
                },
                Name = "TestOne"
            };
            context.Adventures.Add(testAdventure);
            await context.SaveChangesAsync();
            var locationRepository = new LocationsRepository(context);
            
            //act
            var location = await locationRepository.GetInitialForAdventure(72);
        
            //assert
            Assert.Null(location);
        }
        
        [Fact]
        public async Task GetInitialLocation_NoInitial_ReturnNothing()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure
            {
                Locations = new List<Location>()
                {
                    new()
                    {
                        Initial = false
                    },
                    new()
                    {
                        Initial = false
                    }
                },
                Name = "TestOne"
            };
            context.Adventures.Add(testAdventure);
            await context.SaveChangesAsync();
            var locationRepository = new LocationsRepository(context);
            
            //act
            var location = await locationRepository.GetInitialForAdventure(testAdventure.Id);
        
            //assert
            Assert.Null(location);
        }
        
        [Fact]
        public async Task GetInitialLocation_NoLocations_ReturnNothing()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure
            {
                Locations = new List<Location>(),
                Name = "TestOne"
            };
            context.Adventures.Add(testAdventure);
            await context.SaveChangesAsync();
            var locationRepository = new LocationsRepository(context);
            
            //act
            var location = await locationRepository.GetInitialForAdventure(testAdventure.Id);
        
            //assert
            Assert.Null(location);
        }
        
        #endregion
        
        #region GetLocationsForAdventure
        
        [Fact]
        public async Task GetLocationsForAdventure_ReturnsLocations()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure
            {
                Locations = new List<Location>()
                {
                    new()
                    {
                        Initial = true
                    },
                    new()
                    {
                        Initial = false
                    }
                },
                Name = "TestOne"
            };
            var testAdventureTwo = new Adventure
            {
                Locations = new List<Location>()
                {
                    new()
                    {
                        Initial = true
                    }
                },
                Name = "TestTwo"
            };
            context.Adventures.Add(testAdventure);
            context.Adventures.Add(testAdventureTwo);
            await context.SaveChangesAsync();
            var locationRepository = new LocationsRepository(context);
            
            // act
            var locations = await locationRepository.GetLocationsForAdventure(testAdventure.Id);
            
            // assert
            Assert.Equal(2, locations.Count);
        }
        
        #endregion
        
        #region GetLocationById
        
        [Fact]
        public async Task GetLocationById_NotExist_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location"
            };
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            var location = await repository.GetLocationById(testLocation.Id);
            
            // assert
            Assert.Null(location);
        }
        
        [Fact]
        public async Task GetLocationById_Exists_ReturnLocation()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location",
                Adventure = new Adventure()
            };
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            var location = await repository.GetLocationById(testLocation.Id);
            
            // assert
            Assert.NotNull(location);
            Assert.Equal(testLocation.Id, location.Id);
        }
        
        #endregion
        
        #region SaveChanges
        
        [Fact]
        public async Task SaveChanges_ChangesAreSaved()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location"
            };
            context.Locations.Add(testLocation);
            var repository = new LocationsRepository(context);
            
            // act
            await repository.SaveChanges();
            
            // assert
            Assert.Single(context.Locations);
        }
        
        #endregion
        
        #region AddLocation
        
        [Fact]
        public async Task AddLocation_LocationAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location"
            };
            var repository = new LocationsRepository(context);
            
            // act
            await repository.AddLocation(testLocation);
            await repository.SaveChanges();
            
            // assert
            Assert.Single(context.Locations);
        }
        
        #endregion
        
        #region RemoveLocation
        
        [Fact]
        public async Task RemoveLocation_LocationRemoved()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location"
            };
            var testLocationTwo = new Location()
            {
                Name = "test location Two"
            };
            context.Locations.Add(testLocation);
            context.Locations.Add(testLocationTwo);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            repository.RemoveLocation(testLocation);
            await repository.SaveChanges();
            
            // assert
            Assert.Single(context.Locations);
        }
        
        #endregion
        
        #region RemoveLocations
        
        [Fact]
        public async Task RemoveLocations_LocationsRemoved()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location"
            };
            var testLocationTwo = new Location()
            {
                Name = "test location Two"
            };
            context.Locations.Add(testLocation);
            context.Locations.Add(testLocationTwo);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            repository.RemoveLocations(new List<Location>() { testLocation, testLocationTwo});
            await repository.SaveChanges();
            
            // assert
            Assert.Empty(context.Locations);
        }
        
        #endregion
        
        #region GetLocationByIdWithRoutes
        
        [Fact]
        public async Task GetLocationByIdWithRoutes_ValidId_LocationReturnedWithRoutes()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location",
                Adventure = new Adventure(),
                Routes = new List<Route>()
                {
                    new()
                    {
                        Name = "route one"
                    },
                    new()
                    {
                        Name = "route two"
                    }
                }
            };
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            var location = await repository.GetLocationByIdWithRoutes(testLocation.Id);
            
            // assert
            Assert.NotNull(location);
            Assert.Equal(testLocation.Id, location.Id);
            Assert.NotNull(location.Routes);
            Assert.Equal(2, location.Routes.Count);
        }
        
        #endregion
        
        #region GetLocationsWithScript
        
        [Fact]
        public async Task GetLocationsWithScript_HasLocations_ReturnLocations()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location",
                EnterScript = new Script()
                {
                    Name = "test script"
                }
            };
            var testLocationTwo = new Location()
            {
                Name = "test location two",
                EnterScript = testLocation.EnterScript
            };
            var testLocationThree = new Location()
            {
                Name = "test location three"
            };
            await context.Locations.AddRangeAsync(testLocation, testLocationTwo, testLocationThree);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            var locations = await repository.GetLocationsWithScript(testLocation.EnterScript.Id);
            
            // assert
            Assert.Equal(2, locations.Count);
            Assert.Null(locations.FirstOrDefault(loc => loc.Id == testLocationThree.Id));
        }
        
        [Fact]
        public async Task GetLocationsWithScript_NoLocations_ReturnEmpty()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location",
                EnterScript = new Script()
                {
                    Name = "test script"
                }
            };
            var testLocationTwo = new Location()
            {
                Name = "test location two",
                EnterScript = testLocation.EnterScript
            };
            var testLocationThree = new Location()
            {
                Name = "test location three"
            };
            await context.Locations.AddRangeAsync(testLocation, testLocationTwo, testLocationThree);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            var locations = await repository.GetLocationsWithScript(14);
            
            // assert
            Assert.Empty(locations);
        }
        
        #endregion
        
        #region GetAdventureLocationsWithSource
        
        [Fact]
        public async Task GetAdventureLocationsWithSource_NoLocations_ReturnEmpty()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure();
            var testLocation = new Location()
            {
                Adventure = testAdventure,
                SourceKey = Guid.NewGuid()
            };
            var testLocationTwo = new Location()
            {
                Adventure = new Adventure(),
                SourceKey = Guid.NewGuid()
            };
            await context.Adventures.AddRangeAsync(testAdventure);
            await context.Locations.AddRangeAsync(testLocation, testLocationTwo);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            var locations = await repository.GetAdventureLocationsWithSource(
                67, testLocation.SourceKey);
        
            // assert
            Assert.Empty(locations);
        }
        
        [Fact]
        public async Task GetAdventureLocationsWithSource_Exists_ReturnLocations()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure();
            var testLocation = new Location()
            {
                Adventure = testAdventure,
                SourceKey = Guid.NewGuid()
            };
            var testLocationTwo = new Location()
            {
                Adventure = new Adventure(),
                SourceKey = Guid.NewGuid()
            };
            await context.Adventures.AddRangeAsync(testAdventure);
            await context.Locations.AddRangeAsync(testLocation, testLocationTwo);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            var locations = await repository.GetAdventureLocationsWithSource(
                testAdventure.Id, testLocation.SourceKey);
        
            // assert
            Assert.Single(locations);
        }
        
        #endregion
        
        #region GetLocationByIdWithObjects
        
        [Fact]
        public async Task GetLocationByIdWithObjects_ValidId_LocationReturnedWithObjects()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Name = "test location",
                Adventure = new Adventure(),
                AdventureObjects = new List<AdventureObject>()
                {
                    new()
                    {
                        Name = "object1"
                    },
                    new()
                    {
                        Name = "object2"
                    }
                }
            };
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var repository = new LocationsRepository(context);
            
            // act
            var location = await repository.GetLocationByIdWithObjects(testLocation.Id);
            
            // assert
            Assert.NotNull(location);
            Assert.Equal(testLocation.Id, location.Id);
            Assert.NotNull(location.AdventureObjects);
            Assert.Equal(2, location.AdventureObjects.Count);
        }
        
        #endregion
    }
}