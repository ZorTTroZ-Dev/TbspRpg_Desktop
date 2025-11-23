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
    public class GamesServiceTests() : InMemoryTest("GamesServiceTests")
    {
        private static IGamesService CreateService(DatabaseContext context)
        {
            return new GamesService(
                new GameRepository(context),
                NullLogger<GamesService>.Instance);
        }

        #region GetGameByAdventureIdAndUserId

        [Fact]
        public async Task GetGameByAdventureIdAndUserId_Exists_ReturnGame()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure = new Adventure()
            };
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act 
            var game = await service.GetGameByAdventureId(testGame.AdventureId);
            
            // assert
            Assert.NotNull(game);
            Assert.Equal(testGame.Id, game.Id);
        }
        
        [Fact]
        public async Task GetGameByAdventureIdAndUserId_WrongAdventureId_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure = new Adventure()
            };
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act 
            var game = await service.GetGameByAdventureId(17);
            
            // assert
            Assert.Null(game);
        }

        #endregion

        #region AddGame

        [Fact]
        public async Task AddGame_GameAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure = new Adventure()
            };
            var service = CreateService(context);
            
            // act 
            await service.AddGame(testGame);
            await service.SaveChanges();
            
            // assert
            Assert.Single(context.Games);
            Assert.Equal(testGame.Id, context.Games.First().Id);
        }

        #endregion

        #region GetGameByIdIncludeLocation

        [Fact]
        public async Task GetGameByIdIncludeLocation()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocationId = Guid.NewGuid();
            var testGame = new Game()
            {
                Location = new Location()
                {
                    Name = "test location",
                    Initial = true
                }
            };
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var game = await service.GetGameByIdIncludeLocation(testGame.Id);
            
            // assert
            Assert.NotNull(game);
            Assert.NotNull(game.Location);
            Assert.Equal("test location", game.Location.Name);
        }

        #endregion

        #region GetGameByIdIncludeAdventure

        [Fact]
        public async Task GetGameByIdIncludeAdventure()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testLocationId = Guid.NewGuid();
            var testAdventureId = Guid.NewGuid();
            var testGame = new Game()
            {
                Location = new Location()
                {
                    Name = "test location",
                    Initial = true
                },
                Adventure = new Adventure()
                {
                    Name = "test"
                }
            };
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var game = await service.GetGameByIdIncludeAdventure(testGame.Id);
            
            // assert
            Assert.NotNull(game);
            Assert.NotNull(game.Adventure);
            Assert.Equal("test", game.Adventure.Name);
        }

        #endregion

        #region GetGameById

        [Fact]
        public async Task GetGameById_GameExists_ReturnGame()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game();
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var game = await service.GetGameById(testGame.Id);
            
            // assert
            Assert.NotNull(game);
            Assert.Equal(testGame.Id, game.Id);
        }
        
        [Fact]
        public async Task GetGameById_NotExists_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game();
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var game = await service.GetGameById(72);
            
            // assert
            Assert.Null(game);
        }

        #endregion

        #region GetGames

        [Fact]
        public async Task GetGames_NoFilters_ReturnsAll()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure = new Adventure()
            };
            var testGameTwo = new Game()
            {
                Adventure = new Adventure()
            };
            await context.Games.AddAsync(testGame);
            await context.Games.AddAsync(testGameTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var games = await service.GetGames(null);
            
            // assert
            Assert.Equal(2, games.Count);
        }

        #endregion

        #region GetGamesByAdventureId

        [Fact]
        public async Task GetGamesByAdventureId_ValidAdventureId_ReturnsGames()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure = new Adventure()
            };
            var testGameTwo = new Game()
            {
                Adventure = new Adventure()
            };
            await context.Games.AddAsync(testGame);
            await context.Games.AddAsync(testGameTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var games = await service.GetGamesByAdventureId(testGame.AdventureId);
            
            // assert
            Assert.Single(games);
            Assert.Equal(testGame.AdventureId, games[0].AdventureId);
        }

        #endregion

        #region RemoveGame

        [Fact]
        public async Task RemoveGame_GameRemoved()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure = new Adventure()
            };
            var testGameTwo = new Game()
            {
                Adventure = new Adventure()
            };
            await context.Games.AddAsync(testGame);
            await context.Games.AddAsync(testGameTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            service.RemoveGame(testGame);
            await service.SaveChanges();

            // assert
            Assert.Single(context.Games);
        }

        #endregion
    }
}