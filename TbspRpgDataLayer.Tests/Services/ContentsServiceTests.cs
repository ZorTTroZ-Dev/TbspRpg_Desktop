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
    public class ContentsServiceTests() : InMemoryTest("ContentsServiceTests")
    {
        private static IContentsService CreateService(DatabaseContext context)
        {
            return new ContentsService(new ContentsRepository(context),
                NullLogger<ContentsService>.Instance);
        }

        #region AddContent

        [Fact]
        public async Task AddContent_NotExist_ContentAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testContent = new Content()
            {
                Game = new Game(),
                Position = 42
            };
            var service = CreateService(context);
            
            // act
            await service.AddContent(testContent);
            await service.SaveChanges();
            
            // assert
            Assert.Single(context.Contents);
            Assert.Equal(testContent.Id, context.Contents.First().Id);
        }

        [Fact]
        public async Task AddContent_Exists_ContentNotAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testContent = new Content()
            {
                Game = new Game(),
                Position = 42
            };
            context.Contents.Add(testContent);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            await service.AddContent(testContent);
            await service.SaveChanges();
            
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
            var testContent = new Content()
            {
                Game = new Game(),
                Position = 42
            };
            context.Contents.Add(testContent);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var content = await service.GetContentForGameAtPosition(testContent.GameId, 42);
            
            // assert
            Assert.NotNull(content);
            Assert.Equal(testContent.Id, content.Id);
        }

        [Fact]
        public async Task GetContentForGameAtPosition_NotExist_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testContent = new Content()
            {
                Game = new Game(),
                Position = 42
            };
            context.Contents.Add(testContent);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var content = await service.GetContentForGameAtPosition(72, 42);
            
            // assert
            Assert.Null(content);
        }

        #endregion
        
        #region GetAllContent

        [Fact]
        public async Task GetAllContentForGame_GetsAllContent()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetAllContentForGame(game.Id);
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Equal(3, gameContents.Count);
            Assert.Equal(testContents[1].Id, gameContents.First().Id);
        }

        #endregion
        
        #region GetLatestForGame

        [Fact]
        public async Task GetLatestForGame_GetsLatest()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetLatestForGame(game.Id);
            
            //assert
            Assert.Equal(game.Id, gameContents.GameId);
            Assert.Equal(testContents[0].Id, gameContents.Id);
        }

        #endregion
        
        #region GetPartialContentForGame

        [Fact]
        public async Task GetPartialContentForGame_NoDirection_ContentsForward()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(game.Id, new ContentFilterRequest());
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Equal(3, gameContents.Count);
            Assert.Equal(testContents[1].Id, gameContents.First().Id);
        }
        
        [Fact]
        public async Task GetPartialContentForGame_Forward_ContentsForward()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(game.Id, new ContentFilterRequest()
            {
                Direction = "f"
            });
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Equal(3, gameContents.Count);
            Assert.Equal(testContents[1].Id, gameContents.First().Id);
        }
        
        [Fact]
        public async Task GetPartialContentForGame_ForwardStart_PartialContentsForward()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(game.Id, new ContentFilterRequest()
            {
                Direction = "f",
                Start = 2
            });
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Single(gameContents);
            Assert.Equal(testContents[0].Id, gameContents.First().Id);
        }
        
        [Fact]
        public async Task GetPartialContentForGame_ForwardCountStart_PartialContentsForward()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(game.Id, new ContentFilterRequest()
            {
                Direction = "f",
                Start = 1,
                Count = 2
            });
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(testContents[2].Id, gameContents.First().Id);
        }
        
        [Fact]
        public async Task GetPartialContentForGame_ForwardCount_PartialContentsForward()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(game.Id, new ContentFilterRequest()
            {
                Direction = "f",
                Count = 2
            });
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(testContents[1].Id, gameContents.First().Id);
        }
        
        [Fact]
        public async Task GetPartialContentForGame_Backward_ContentsBackward()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(game.Id, new ContentFilterRequest()
            {
                Direction = "b"
            });
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Equal(3, gameContents.Count);
            Assert.Equal(testContents[0].Id, gameContents.First().Id);
        }
        
        [Fact]
        public async Task GetPartialContentForGame_BackwardStart_PartialContentsBackward()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(game.Id, new ContentFilterRequest()
            {
                Direction = "b",
                Start = 1
            });
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(testContents[2].Id, gameContents.First().Id);
        }
        
        [Fact]
        public async Task GetPartialContentForGame_BackwardCount_PartialContentsBackward()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(game.Id, new ContentFilterRequest()
            {
                Direction = "b",
                Count = 2
            });
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(testContents[0].Id, gameContents.First().Id);
        }
        
        [Fact]
        public async Task GetPartialContentForGame_BackwardStartCount_PartialContentsBackward()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(game.Id, new ContentFilterRequest()
            {
                Direction = "b",
                Start = 1,
                Count = 2
            });
            
            //assert
            Assert.Equal(game.Id, gameContents.First().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(testContents[1].Id, gameContents[1].Id);
        }
        
        [Fact]
        public async Task GetPartialContentForGame_BadDirection_Error()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var game = new Game();
            var testContents = new List<Content>()
            {
                new()
                {
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetPartialContentForGame(game.Id, new ContentFilterRequest()
                {
                    Direction = "zebra",
                    Start = -3,
                    Count = 2
                }));
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
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var contents = await service.GetContentForGameAfterPosition(game.Id, 40);
            
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
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            //act
            var contents = await service.GetContentForGameAfterPosition(game.Id, 42);
            
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
                    Game = game,
                    Position = 42
                },
                new()
                {
                    Game = game,
                    Position = 0
                },
                new()
                {
                    Game = game,
                    Position = 1
                }
            };
            context.Contents.AddRange(testContents);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            // act
            service.RemoveContents(testContents);
            
            // assert
            await service.SaveChanges();
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
            var service = CreateService(context);
            
            // act
            var contents = await service.GetAdventureContentsWithSource(
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
            var service = CreateService(context);
            
            // act
            var contents = await service.GetAdventureContentsWithSource(
                testContent.Game.AdventureId, Guid.NewGuid());
            
            // assert
            Assert.Empty(contents);
        }

        #endregion

        #region DoesAdventureContentUseSource

        [Fact]
        public async Task DoesAdventureContentUseSource_UsesSource_ReturnTrue()
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
            var service = CreateService(context);
            
            // act
            var usesSource = await service.DoesAdventureContentUseSource(
                testContent.Game.AdventureId, testContent.SourceKey);
            
            // assert
            Assert.True(usesSource);
        }

        [Fact]
        public async Task DoesAdventurecontentUseSource_NoUseSource_ReturnFalse()
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
            var service = CreateService(context);
            
            // act
            var usesSource = await service.DoesAdventureContentUseSource(
                74, testContent.SourceKey);
            
            // assert
            Assert.False(usesSource);
        }

        #endregion
    }
}