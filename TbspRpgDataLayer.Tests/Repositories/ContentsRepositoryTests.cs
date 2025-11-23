using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using Xunit;

namespace TbspRpgDataLayer.Tests.Repositories
{
    public class ContentsRepositoryTests() : InMemoryTest("ContentsRepositoryTests")
    {
        private static IContentsRepository CreateRepository(DatabaseContext context)
        {
            return new ContentsRepository(context);
        }

        #region AddContent

        [Fact]
        public async Task AddContent_ContentAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testContent = new Content()
            {
                Position = 42
            };
            var repository = CreateRepository(context);
            
            // act
            await repository.AddContent(testContent);
            await repository.SaveChanges();
            
            // assert
            Assert.Single(context.Contents);
            Assert.Equal(testContent.Id, context.Contents.First().Id);
        }

        #endregion

        #region GetContentForGameAtPosition

        [Fact]
        public async Task GetContentForGameAtPosition_Exists_ReturnContent()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContent = new Content()
            {
                Position = 42,
                Game = new Game()
            };
            context.Contents.Add(testContent);
            await context.SaveChangesAsync();
            var repository = CreateRepository(context);
            
            // act
            var content = await repository.GetContentForGameAtPosition(testContent.GameId, 42);
            
            // assert
            Assert.NotNull(content);
            Assert.Equal(testContent.Id, content.Id);
        }
        
        [Fact]
        public async Task GetContentForGameAtPosition_InvalidGame_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testContent = new Content()
            {
                Position = 42,
                Game = new Game()
            };
            context.Contents.Add(testContent);
            await context.SaveChangesAsync();
            var repository = CreateRepository(context);
            
            // act
            var content = await repository.GetContentForGameAtPosition(74, 42);
            
            // assert
            Assert.Null(content);
        }
        
        [Fact]
        public async Task GetContentForGameAtPosition_InvalidPosition_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testContent = new Content()
            {
                Position = 42,
                Game = new Game()
            };
            context.Contents.Add(testContent);
            await context.SaveChangesAsync();
            var repository = CreateRepository(context);
            
            // act
            var content = await repository.GetContentForGameAtPosition(testContent.GameId, 40);
            
            // assert
            Assert.Null(content);
        }

        #endregion

        #region GetContentForGame

        [Fact]
        public async Task GetContentForGame_NoCountNoOffset_ReturnsAll()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGame(game.Id);
            
            //assert
            Assert.Equal(3, contents.Count);
            Assert.Equal((ulong)0, contents[0].Position);
            Assert.Equal((ulong)42, contents[2].Position);
        }
        
        [Fact]
        public async Task GetContentForGame_NoOffset_ReturnPartial()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGame(game.Id, null, 2);
            
            //assert
            Assert.Equal(2, contents.Count);
            Assert.Equal((ulong)0, contents[0].Position);
            Assert.Equal((ulong)1, contents[1].Position);
        }
        
        [Fact]
        public async Task GetContentForGame_NoCount_ReturnPartial()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGame(game.Id, 2);
            
            //assert
            Assert.Single(contents);
            Assert.Equal(testContents[0].Id, contents[0].Id);
            Assert.Equal((ulong)42, contents[0].Position);
        }
        
        [Fact]
        public async Task GetContentForGame_OffsetCount_ReturnPartial()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGame(game.Id, 1, 2);
            
            //assert
            Assert.Equal(2, contents.Count);
            Assert.Equal((ulong)1, contents[0].Position);
            Assert.Equal((ulong)42, contents[1].Position);
        }
        
        #endregion
        
        #region GetContentForGameReverse

        [Fact]
        public async Task GetContentForGameReverse_NoCountNoOffset_ReturnsAll()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGameReverse(game.Id);
            
            //assert
            Assert.Equal(3, contents.Count);
            Assert.Equal((ulong)42, contents[0].Position);
            Assert.Equal((ulong)0, contents[2].Position);
        }
        
        [Fact]
        public async Task GetContentForGameReverse_NoOffset_ReturnPartial()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGameReverse(game.Id, null, 2);
            
            //assert
            Assert.Equal(2, contents.Count);
            Assert.Equal((ulong)42, contents[0].Position);
            Assert.Equal((ulong)1, contents[1].Position);
        }

        [Fact]
        public async Task GetContentForGameReverse_NoCount_ReturnPartial()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGameReverse(game.Id, 2);
            
            //assert
            Assert.Single(contents);
            Assert.Equal((ulong)0, contents[0].Position);
        }
        
        [Fact]
        public async Task GetContentForGameReverse_OffsetCount_ReturnPartial()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGameReverse(game.Id, 1, 2);
            
            //assert
            Assert.Equal(2, contents.Count);
            Assert.Equal((ulong)1, contents[0].Position);
            Assert.Equal((ulong)0, contents[1].Position);
        }
        
        #endregion
        
        #region GetContentForGameAfterPosition

        [Fact]
        public async Task GetContentForGameAfterPosition_EarlyPosition_ReturnContent()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGameAfterPosition(game.Id, 40);
            
            //assert
            Assert.Single(contents);
            Assert.Equal(testContents[0].Id, contents[0].Id);
        }
        
        [Fact]
        public async Task GetContentForGameAfterPosition_LastPosition_ReturnNoContent()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            //act
            var contents = await repository.GetContentForGameAfterPosition(game.Id, 42);
            
            //assert
            Assert.Empty(contents);
        }

        #endregion

        #region RemoveContents

        [Fact]
        public async Task RemoveContents_ContentsRemoved()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            // act
            repository.RemoveContents(testContents);
            
            // assert
            await repository.SaveChanges();
            Assert.Empty(context.Contents);
        }

        #endregion
        
        #region RemoveAllContentsForGame

        [Fact]
        public async Task RemoveAllContentsForGame_NoContents_NothingRemoved()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>();
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            // act
            await repository.RemoveAllContentsForGame(game.Id);
            await repository.SaveChanges();
            
            // assert
            Assert.Empty(context.Contents);
        }
        
        [Fact]
        public async Task RemoveAllContentsForGame_InvalidGameId_NothingRemoved()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            // act
            await repository.RemoveAllContentsForGame(74);
            await repository.SaveChanges();
            
            // assert
            Assert.Equal(3, context.Contents.Count());
        }
        
        [Fact]
        public async Task RemoveAllContentsForGame_Valid_ContentsRemoved()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Position = 42,
                    Game = game
                },
                new()
                {
                    Position = 0,
                    Game = game
                },
                new()
                {
                    Position = 1,
                    Game = game
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var repository = new ContentsRepository(context);
            
            // act
            await repository.RemoveAllContentsForGame(game.Id);
            await repository.SaveChanges();
            
            // assert
            Assert.Empty(context.Contents);
        }

        #endregion

        #region GetAdventureContentsWithSource

        [Fact]
        public async Task GetAdventureContentsWithSource_HasSource_ReturnsContents()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testContent = new Content()
            {
                SourceKey = Guid.NewGuid(),
                Game = new Game()
                {
                    Adventure = new Adventure()
                }
            };
            await context.Contents.AddAsync(testContent);
            await context.SaveChangesAsync();
            var repository = CreateRepository(context);
            
            // act
            var contents = await repository.GetAdventureContentsWithSource(
                testContent.Game.AdventureId, testContent.SourceKey);
            
            // assert
            Assert.Single(contents);
            Assert.Equal(testContent.SourceKey, contents[0].SourceKey);
        }

        [Fact]
        public async Task GetAdventureContentsWithSource_NoHasSource_ReturnsEmptyList()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testContent = new Content()
            {
                SourceKey = Guid.NewGuid(),
                Game = new Game()
                {
                    Adventure = new Adventure()
                }
            };
            await context.Contents.AddAsync(testContent);
            await context.SaveChangesAsync();
            var repository = CreateRepository(context);
            
            // act
            var contents = await repository.GetAdventureContentsWithSource(
                testContent.Game.AdventureId, Guid.NewGuid());
            
            // assert
            Assert.Empty(contents);
        }

        #endregion
    }
}