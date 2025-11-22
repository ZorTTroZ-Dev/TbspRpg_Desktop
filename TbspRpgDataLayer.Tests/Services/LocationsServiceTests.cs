using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;
using Xunit;

namespace TbspRpgDataLayer.Tests.Services
{
    public class LocationsServiceTests() : InMemoryTest("LocationsServiceTests")
    {
        private static ILocationsService CreateService(DatabaseContext context)
        {
            return new LocationsService(new LocationsRepository(context),
                NullLogger<LocationsService>.Instance);
        }

        #region GetInitialLocationForAdventure

        [Fact]
        public async Task GetInitialLocationForAdventure_Valid_ReturnLocation()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Adventure = new Adventure(),
                Initial = true
            };
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var location = await service.GetInitialLocationForAdventure(testLocation.AdventureId);
            
            // assert
            Assert.NotNull(location);
            Assert.Equal(testLocation.Id, location.Id);
        }
        
        [Fact]
        public async Task GetInitialLocationForAdventure_InvalidAdventure_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Adventure = new Adventure(),
                Initial = true
            };
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var location = await service.GetInitialLocationForAdventure(78);
            
            // assert
            Assert.Null(location);
        }
        
        [Fact]
        public async Task GetInitialLocationForAdventure_NoInitial_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Adventure = new Adventure(),
                Initial = false
            };
            context.Locations.Add(testLocation);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var location = await service.GetInitialLocationForAdventure(testLocation.AdventureId);
            
            // assert
            Assert.Null(location);
        }

        #endregion

        #region GetLocationsForAdventure

        [Fact]
        public async Task GetLocationsForAdventure_ReturnsLocations()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocation = new Location()
            {
                Adventure = new Adventure(),
                Initial = false
            };
            var testLocationTwo = new Location()
            {
                Adventure = new Adventure(),
                Initial = false
            };
            context.Locations.Add(testLocation);
            context.Locations.Add(testLocationTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var locations = await service.GetLocationsForAdventure(testLocation.AdventureId);
            
            // assert
            Assert.Single(locations);
            Assert.Equal(testLocation.Id, locations[0].Id);
        }

        #endregion

        #region GetLocationById

        [Fact]
        public async Task GetLocationById_Exists_ReturnLocation()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var location = new Location()
            {
                Name = "test location",
                Adventure = new Adventure()
            };
            context.Locations.Add(location);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var dblocation = await service.GetLocationById(location.Id);
            
            // assert
            Assert.NotNull(dblocation);
            Assert.Equal(location.Id, dblocation.Id);
        }

        #endregion

        #region SaveChanges

        [Fact]
        public async Task SaveChanges_SavesChanges()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var location = new Location()
            {
                Name = "test location"
            };
            context.Locations.Add(location);
            var service = CreateService(context);
            
            // act
            await service.SaveChanges();
            
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
            var service = CreateService(context);
            
            // act
            await service.AddLocation(testLocation);
            await service.SaveChanges();
            
            // assert
            Assert.Single(context.Locations);
        }
        
        #endregion

        #region RemoveScriptFromLocations

        [Fact]
        public async Task RemoveScriptFromLocations_ScriptRemoved()
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
            var service = CreateService(context);
            
            // act
            await service.RemoveScriptFromLocations(testLocation.EnterScript.Id);
            await service.SaveChanges();
            
            // assert
            Assert.Null(context.Locations.First(loc => loc.Id == testLocation.Id).EnterScript);
            Assert.Null(context.Locations.First(loc => loc.Id == testLocationTwo.Id).ExitScript);
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
            var service = CreateService(context);
            
            // act
            var locations = await service.GetAdventureLocationsWithSource(82, testLocation.SourceKey);

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
            var service = CreateService(context);
            
            // act
            var locations = await service.GetAdventureLocationsWithSource(
                testAdventure.Id, testLocation.SourceKey);

            // assert
            Assert.Single(locations);
        }

        #endregion

        #region DoesAdventureLocationUseSource

        [Fact]
        public async Task DoesAdventLocationUseSource_DoesUseSource_ReturnTrue()
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
            var service = CreateService(context);
            
            // act
            var usesSource = await service.DoesAdventureLocationUseSource(
                testAdventure.Id, testLocation.SourceKey);
            
            // assert
            Assert.True(usesSource);
        }
        
        [Fact]
        public async Task DoesAdventLocationUseSource_DoesntUseSource_ReturnFalse()
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
            var service = CreateService(context);
            
            // act
            var usesSource = await service.DoesAdventureLocationUseSource(86, testLocation.SourceKey);
            
            // assert
            Assert.False(usesSource);
        }

        #endregion
    }
}