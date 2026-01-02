using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgDataLayer.Tests.Repositories
{
    public class AdventuresRepositoryTests() : InMemoryTest("AdventuresRepositoryTests")
    {
        #region GetAllAdventures
        
        [Fact]
        public async Task GetAllAdventures_ReturnAll()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Name = "TestOne"
            };
            var testAdventureTwo = new Adventure()
            {
                Name = "TestTwo"
            };
            context.Adventures.AddRange(testAdventure, testAdventureTwo);
            await context.SaveChangesAsync();
            var adventureRepository = new AdventuresRepository(context);
            
            //act
            var adventures = await adventureRepository.GetAllAdventures(new AdventureFilter());
            
            //assert
            Assert.Equal(2, adventures.Count);
            Assert.Equal("TestOne", adventures[0].Name);
            Assert.Equal("TestTwo", adventures[1].Name);
        }
        
        #endregion
        
        #region GetAdventureByName
        
        [Fact]
        public async Task GetAdventureByName_ReturnAdventure()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Name = "TestOne"
            };
            context.Adventures.AddRange(testAdventure);
            await context.SaveChangesAsync();
            var adventureRepository = new AdventuresRepository(context);
            
            //act
            var adventure = await adventureRepository.GetAdventureByName("TestOne");
            
            //assert
            Assert.Equal("TestOne", adventure.Name);
        }
        
        [Fact]
        public async Task GetAdventureByName_CaseInsensitive_ReturnAdventure()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Name = "TestOne"
            };
            var testAdventureTwo = new Adventure()
            {
                Name = "TestTwo"
            };
            context.Adventures.AddRange(testAdventure, testAdventureTwo);
            await context.SaveChangesAsync();
            var adventureRepository = new AdventuresRepository(context);
            
            //act
            var adventure = await adventureRepository.GetAdventureByName("testTWO");
            
            //assert
            Assert.Equal("TestTwo", adventure.Name);
        }
        
        [Fact]
        public async Task GetAdventureByName_InvalidName_ReturnNothing()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Name = "TestOne"
            };
            context.Adventures.AddRange(testAdventure);
            await context.SaveChangesAsync();
            var adventureRepository = new AdventuresRepository(context);
            
            //act
            var adventure = await adventureRepository.GetAdventureByName("invalid");
            
            //assert
            Assert.Null(adventure);
        }
        
        #endregion
        
        #region GetAdventureById
        
        [Fact]
        public async Task GetAdventureById_ReturnAdventure()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Name = "TestOne"
            };
            context.Adventures.AddRange(testAdventure);
            await context.SaveChangesAsync();
            var adventureRepository = new AdventuresRepository(context);
            
            //act
            var adventure = await adventureRepository.GetAdventureById(testAdventure.Id);
            
            //assert
            Assert.Equal("TestOne", adventure.Name);
        }
        
        [Fact]
        public async Task GetAdventureById_Invalid_ReturnNothing()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Name = "TestOne"
            };
            context.Adventures.AddRange(testAdventure);
            await context.SaveChangesAsync();
            var adventureRepository = new AdventuresRepository(context);
            
            //act
            var adventure = await adventureRepository.GetAdventureById(42);
            
            //assert
            Assert.Null(adventure);
        }
        
        #endregion

        #region AddAdventure

        [Fact]
        public async Task AddAdventure_AdventureAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var newAdventure = new Adventure()
            {
                Name = "test_adventure",
                InitialCopyKey = Guid.Empty
            };
            var repository = new AdventuresRepository(context);
            
            // act
            await repository.AddAdventure(newAdventure);
            await repository.SaveChanges();
            
            // assert
            Assert.Single(context.Adventures);
            Assert.Equal("test_adventure", context.Adventures.First().Name);
        }

        #endregion

        #region GetAdventureByIdIncludeAssociatedObjects

        [Fact]
        public async Task GetAdventureByIdIncludeAssociatedObjects_Valid_ReturnAdventureWithEverything()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var location = new Location()
            {
                Initial = true,
                Routes = new List<Route>()
                {
                    new()
                }
            };
            var newAdventure = new Adventure()
            {
                Name = "test_adventure",
                InitialCopyKey = Guid.Empty,
                Games = new List<Game>()
                {
                    new()
                },
                Locations = new List<Location>()
                {
                    location
                }
            };
            await context.AddAsync(newAdventure);
            await context.SaveChangesAsync();
            var repository = new AdventuresRepository(context);
            
            // act
            var adventure = await repository.GetAdventureByIdIncludeAssociatedObjects(newAdventure.Id);

            // assert
            Assert.NotNull(adventure);
            Assert.NotNull(adventure.Games);
            Assert.Single(adventure.Games);
            Assert.NotNull(adventure.Locations);
            Assert.Single(adventure.Locations);
            Assert.NotNull(adventure.Locations.First().Routes);
            Assert.Single(adventure.Locations.First().Routes);
        }

        #endregion
        
        #region RemoveAdventure

        [Fact]
        public async Task RemoveAdventure_Valid_AdventureRemoved()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var newAdventure = new Adventure()
            {
                Name = "test_adventure",
                InitialCopyKey = Guid.Empty
            };
            await context.AddAsync(newAdventure);
            await context.SaveChangesAsync();
            var repository = new AdventuresRepository(context);
            
            // act
            repository.RemoveAdventure(newAdventure);
            await repository.SaveChanges();
            
            // assert
            Assert.Empty(context.Adventures);
        }

        #endregion

        #region GetAdventuresWithScript

        [Fact]
        public async Task GetAdventuresWithScript_HasScripts_AdventuresReturned()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testScript = new Script()
            {
                Content = "banana",
                Name = "test",
                Type = ScriptTypes.LuaScript
            };
            var newAdventure = new Adventure()
            {
                Name = "test_adventure",
                InitialCopyKey = Guid.Empty,
                InitializationScript = testScript 
            };
            var newAdventureTwo = new Adventure()
            {
                Name = "test_adventure_two",
                TerminationScript = testScript
            };
            var newAdventureThree = new Adventure()
            {
                Name = "test_adventure_three"
            };
            await context.AddRangeAsync(newAdventure, newAdventureTwo, newAdventureThree);
            await context.SaveChangesAsync();
            var repository = new AdventuresRepository(context);
            
            // act
            var adventures = await repository.GetAdventuresWithScript(testScript.Id);
            
            // assert
            Assert.Equal(2, adventures.Count);
        }

        [Fact]
        public async Task GetAdventuresWithScript_NoScripts_ReturnEmpty()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testScript = new Script()
            {
                Content = "banana",
                Name = "test",
                Type = ScriptTypes.LuaScript
            };
            var newAdventure = new Adventure()
            {
                Name = "test_adventure",
                InitialCopyKey = Guid.Empty,
                InitializationScript = testScript 
            };
            var newAdventureTwo = new Adventure()
            {
                Name = "test_adventure_two",
                TerminationScript = testScript
            };
            var newAdventureThree = new Adventure()
            {
                Name = "test_adventure_three"
            };
            await context.AddRangeAsync(newAdventure, newAdventureTwo, newAdventureThree);
            await context.SaveChangesAsync();
            var repository = new AdventuresRepository(context);
            
            // act
            var adventures = await repository.GetAdventuresWithScript(42);
            
            // assert
            Assert.Empty(adventures);
        }

        #endregion

        #region GetAdventureWithSource

        [Fact]
        public async Task GetAdventureWithSource_DoesntExist_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                DescriptionCopyKey = Guid.NewGuid(),
                InitialCopyKey = Guid.NewGuid()
            };
            await context.AddRangeAsync(testAdventure);
            await context.SaveChangesAsync();
            var repository = new AdventuresRepository(context);
            
            // act
            var adventure = await repository.GetAdventureWithSource(42, Guid.NewGuid());
            
            // assert
            Assert.Null(adventure);
        }

        [Fact]
        public async Task GetAdventureWithSource_DescriptionSourceKey_ReturnAdventure()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                DescriptionCopyKey = Guid.NewGuid(),
                InitialCopyKey = Guid.NewGuid()
            };
            await context.AddRangeAsync(testAdventure);
            await context.SaveChangesAsync();
            var repository = new AdventuresRepository(context);
            
            // act
            var adventure =
                await repository.GetAdventureWithSource(testAdventure.Id, testAdventure.DescriptionCopyKey);
            
            // assert
            Assert.NotNull(adventure);
            Assert.Equal(testAdventure.Id, adventure.Id);
        }

        [Fact]
        public async Task GetAdventureWithSource_InitialSourceKey_ReturnAdventure()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                DescriptionCopyKey = Guid.NewGuid(),
                InitialCopyKey = Guid.NewGuid()
            };
            await context.AddRangeAsync(testAdventure);
            await context.SaveChangesAsync();
            var repository = new AdventuresRepository(context);
            
            // act
            var adventure =
                await repository.GetAdventureWithSource(testAdventure.Id, testAdventure.InitialCopyKey);
            
            // assert
            Assert.NotNull(adventure);
            Assert.Equal(testAdventure.Id, adventure.Id);
        }

        #endregion
    }
}