using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor.Entities;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgProcessor.Tests.Processors
{
    public class ContentProcessorTests : ProcessorTest
    {
        #region GetContentTextForKey

        [Fact]
        public async Task GetContentTextForKey_NoGame_ThrowException()
        {
            // arrange
            var testGame = new Game()
            {
                Id = 1,
                Language = Languages.ENGLISH
            };
            var testSource = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                Text = "test source"
            };
            var testGames = new List<Game>() {testGame};
            var testSources = new List<En>() {testSource};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Sources = testSources,
                Games = testGames
            });

            // act
            Task Act() => processor.GetContentTextForKey(new ContentTextForKeyModel()
            {
                GameId = 17,
                SourceKey = testSource.Key
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task GetContentTextForKey_NullLanguage_SourceReturned()
        {
            // arrange
            var adventureId = 1;
            var testGame = new Game()
            {
                Id = 1,
                AdventureId = adventureId
            };
            var testSource = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = adventureId,
                Text = "test source"
            };
            var testGames = new List<Game>() {testGame};
            var testSources = new List<En>() {testSource};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Sources = testSources,
                Games = testGames
            });
            
            // act
            var source = await processor.GetContentTextForKey(new ContentTextForKeyModel()
            {
                GameId = testGame.Id,
                SourceKey = testSource.Key
            });
            
            // assert
            Assert.NotNull(source);
            Assert.Equal("test source", source);
        }

        [Fact]
        public async Task GetContentTextForKey_Valid_SourceReturned()
        {
            // arrange
            var adventureId = 1;
            var testGame = new Game()
            {
                Id = 1,
                AdventureId = adventureId,
                Language = Languages.ENGLISH
            };
            var testSource = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = adventureId,
                Text = "test source"
            };
            var testGames = new List<Game>() {testGame};
            var testSources = new List<En>() {testSource};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData()
            {
                Sources = testSources,
                Games = testGames
            });
            
            // act
            var source = await processor.GetContentTextForKey(new ContentTextForKeyModel()
            {
                GameId = testGame.Id,
                SourceKey = testSource.Key
            });
            
            // assert
            Assert.NotNull(source);
            Assert.Equal("test source", source);
        }

        #endregion
    }
}