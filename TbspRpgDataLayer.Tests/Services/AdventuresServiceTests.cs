using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgDataLayer.Tests.Services
{
    public class AdventuresServiceTests() : InMemoryTest("AdventuresServiceTests")
    {
        private static IAdventuresService CreateService(DatabaseContext context)
        {
            return new AdventuresService(
                new AdventuresRepository(context),
                NullLogger<AdventuresService>.Instance);
        }
        
        #region GetAllAdventures

        [Fact]
        public async Task GetAllAdventures_AllAdventuresReturned()
        {
            //  arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "TestOne"
            };
            var testAdventureTwo = new Adventure()
            {
                Id = 2,
                Name = "TestTwo"
            };
            context.Adventures.AddRange(testAdventure, testAdventureTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var adventures = await service.GetAllAdventures(new AdventureFilter());
            
            // assert
            Assert.Equal(2, adventures.Count);
            Assert.Equal(testAdventure.Id, adventures.First().Id);
        }
        
        #endregion

        #region GetAdventureByName

        [Fact]
        public async Task GetAdventureByName_Exact_ReturnAdventure()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            };
            context.Adventures.Add(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var adventure = await service.GetAdventureByName("test");
            
            // assert
            Assert.NotNull(adventure);
            Assert.Equal(testAdventure.Id, adventure.Id);
        }

        [Fact]
        public async Task GetAdventureByName_CaseInsensitive_ReturnAdventure()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            };
            context.Adventures.Add(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var adventure = await service.GetAdventureByName("tEsT");
            
            // assert
            Assert.NotNull(adventure);
            Assert.Equal(testAdventure.Id, adventure.Id);
        }

        [Fact]
        public async Task GetAdventureByName_Invalid_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            };
            context.Adventures.Add(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var adventure = await service.GetAdventureByName("testy");
            
            // assert
            Assert.Null(adventure);
        }

        #endregion

        #region GetAdventureById

        [Fact]
        public async Task GetAdventureById_Exists_ReturnAdventure()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            };
            context.Adventures.Add(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var adventure = await service.GetAdventureById(testAdventure.Id);
            
            // assert
            Assert.NotNull(adventure);
            Assert.Equal(testAdventure.Id, adventure.Id);
        }

        [Fact]
        public async Task GetAdventureById_Invalid_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "test"
            };
            context.Adventures.Add(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var adventure = await service.GetAdventureById(42);
            
            // assert
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
            var service = CreateService(context);
        
            // act
            await service.AddAdventure(newAdventure);
            await service.SaveChanges();
        
            // assert
            Assert.Single(context.Adventures);
            Assert.Equal("test_adventure", context.Adventures.First().Name);
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
            var service = CreateService(context);
            
            // act
            service.RemoveAdventure(newAdventure);
            await service.SaveChanges();
            
            // assert
            Assert.Empty(context.Adventures);
        }

        #endregion

        #region RemoveScriptFromAdventures

        [Fact]
        public async Task RemoveScriptFromAdventures_ScriptRemoved()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Id = 1,
                Name = "test",
                InitializationScript = new Script()
                {
                    Id = 1,
                    Name = "test",
                    Type = ScriptTypes.LuaScript,
                    Content = "banana"
                }
            };
            var testAdventureTwo = new Adventure()
            {
                Id = 2,
                Name = "test two",
                TerminationScript = testAdventure.InitializationScript
            };
            await context.AddRangeAsync(testAdventure, testAdventureTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            await service.RemoveScriptFromAdventures(testAdventure.InitializationScript.Id);
            await service.SaveChanges();
            
            // assert
            Assert.Null(context.Adventures.First(a => a.Id == testAdventure.Id).InitializationScript);
            Assert.Null(context.Adventures.First(a => a.Id == testAdventureTwo.Id).TerminationScript);
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
                Id = 1,
                DescriptionCopyKey = Guid.NewGuid(),
                InitialCopyKey = Guid.NewGuid()
            };
            await context.AddRangeAsync(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var adventure = await service.GetAdventureWithSource(42, Guid.NewGuid());
            
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
                Id = 1,
                DescriptionCopyKey = Guid.NewGuid(),
                InitialCopyKey = Guid.NewGuid()
            };
            await context.AddRangeAsync(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var adventure =
                await service.GetAdventureWithSource(testAdventure.Id, testAdventure.DescriptionCopyKey);
            
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
                Id = 1,
                DescriptionCopyKey = Guid.NewGuid(),
                InitialCopyKey = Guid.NewGuid()
            };
            await context.AddRangeAsync(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var adventure =
                await service.GetAdventureWithSource(testAdventure.Id, testAdventure.InitialCopyKey);
            
            // assert
            Assert.NotNull(adventure);
            Assert.Equal(testAdventure.Id, adventure.Id);
        }

        #endregion

        #region DoesAdventureUseSource

        [Fact]
        public async Task DoesAdventureUseSource_DoesntUseSource_ReturnFalse()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Id = 1,
                DescriptionCopyKey = Guid.NewGuid(),
                InitialCopyKey = Guid.NewGuid()
            };
            await context.AddRangeAsync(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var useSource = await service.DoesAdventureUseSource(testAdventure.Id, Guid.NewGuid());
            
            // assert
            Assert.False(useSource);
        }

        [Fact]
        public async Task DoesAdventureUseSource_UsesSource_ReturnTrue()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testAdventure = new Adventure()
            {
                Id = 1,
                DescriptionCopyKey = Guid.NewGuid(),
                InitialCopyKey = Guid.NewGuid()
            };
            await context.AddRangeAsync(testAdventure);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var useSource = await service.DoesAdventureUseSource(testAdventure.Id, testAdventure.DescriptionCopyKey);
            
            // assert
            Assert.True(useSource);
        }

        #endregion
    }
}