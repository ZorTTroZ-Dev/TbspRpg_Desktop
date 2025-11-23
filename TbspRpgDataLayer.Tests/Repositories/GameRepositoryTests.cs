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
    public class GameRepositoryTests() : InMemoryTest("GameRepositoryTests")
    {
        #region GetGameById

        [Fact]
        public async Task GetGameById_Valid_ReturnGame()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game();
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var gameRepository = new GameRepository(context);
            
            // act
            var game = await gameRepository.GetGameById(testGame.Id);
            
            // assert
            Assert.NotNull(game);
            Assert.Equal(testGame.Id, game.Id);
        }

        [Fact]
        public async Task GetGameById_Invalid_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game();
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var gameRepository = new GameRepository(context);
            
            // act
            var game = await gameRepository.GetGameById(42);
            
            // assert
            Assert.Null(game);
        }

        #endregion

        #region GetGameByAdventureIdAndUserId

        [Fact]
        public async Task GetGameByAdventureIdAndUserId_Valid_ReturnGame()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure = new Adventure()
            };
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var gameRepository = new GameRepository(context);
            
            // act
            var game = await gameRepository.GetGameByAdventureId(testGame.AdventureId);
            
            // assert
            Assert.NotNull(game);
            Assert.Equal(testGame.Id, game.Id);
        }
        
        [Fact]
        public async Task GetGameByAdventureIdAndUserId_InValidAdventureId_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure = new Adventure()
            };
            context.Games.Add(testGame);
            await context.SaveChangesAsync();
            var gameRepository = new GameRepository(context);
            
            // act
            var game = await gameRepository.GetGameByAdventureId(13);
            
            // assert
            Assert.Null(game);
        }

        #endregion

        #region AddGame

        [Fact]
        public async Task AddGame_Valid_GameAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var gameRepository = new GameRepository(context);

            // act
            await gameRepository.AddGame(game);
            await context.SaveChangesAsync();
            
            // assert
            Assert.Single(context.Games);
            Assert.NotNull(context.Games.AsQueryable().FirstOrDefault());
            // ReSharper disable once PossibleNullReferenceException
            Assert.Equal(game.Id, context.Games.AsQueryable().First().Id);
        }

        #endregion

        #region GetGameByIdIncludeLocation

        [Fact]
        public async Task GetGameByIdIncludeLocation()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
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
            var repository = new GameRepository(context);
            
            // act
            var game = await repository.GetGameByIdWithLocation(testGame.Id);
            
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
            var repository = new GameRepository(context);
            
            // act
            var game = await repository.GetGameByIdWithAdventure(testGame.Id);
            
            // assert
            Assert.NotNull(game);
            Assert.NotNull(game.Adventure);
            Assert.Equal("test", game.Adventure.Name);
        }

        #endregion

        #region GetGames

        [Fact]
        public async Task GetGames_FilterByAdventureId_ReturnsGames()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure =  new Adventure()
            };
            var testGameTwo = new Game()
            {
                Adventure =  new Adventure()
            };
            await context.Games.AddAsync(testGame);
            await context.Games.AddAsync(testGameTwo);
            await context.SaveChangesAsync();
            var repository = new GameRepository(context);
            
            // act
            var games = await repository.GetGames(new GameFilter()
            {
                AdventureId = testGame.AdventureId
            });
            
            // assert
            Assert.Single(games);
            Assert.Equal(testGame.AdventureId, games[0].AdventureId);
        }

        [Fact]
        public async Task GetGames_NoFilter_ReturnsAll()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure =  new Adventure()
            };
            var testGameTwo = new Game()
            {
                Adventure =  new Adventure()
            };
            await context.Games.AddAsync(testGame);
            await context.Games.AddAsync(testGameTwo);
            await context.SaveChangesAsync();
            var repository = new GameRepository(context);
            
            // act
            var games = await repository.GetGames(null);
            
            // assert
            Assert.Equal(2, games.Count);
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
                Adventure =  new Adventure()
            };
            var testGameTwo = new Game()
            {
                Adventure =  new Adventure()
            };
            await context.Games.AddAsync(testGame);
            await context.Games.AddAsync(testGameTwo);
            await context.SaveChangesAsync();
            var repository = new GameRepository(context);
            
            // act
            repository.RemoveGame(testGame);
            await repository.SaveChanges();

            // assert
            Assert.Single(context.Games);
        }

        #endregion
        
        #region RemoveGames

        [Fact]
        public async Task RemoveGames_GamesRemoved()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testGame = new Game()
            {
                Adventure =  new Adventure()
            };
            var testGameTwo = new Game()
            {
                Adventure =  new Adventure()
            };
            await context.Games.AddAsync(testGame);
            await context.Games.AddAsync(testGameTwo);
            await context.SaveChangesAsync();
            var repository = new GameRepository(context);
            
            // act
            repository.RemoveGames(new List<Game>() { testGame, testGameTwo});
            await repository.SaveChanges();

            // assert
            Assert.Empty(context.Games);
        }

        #endregion
    }
}