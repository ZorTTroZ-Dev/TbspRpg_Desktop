using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgDataLayer.Tests.Services
{
    public class SourcesServiceTests() : InMemoryTest("SourcesServiceTests")
    {
        private static ISourcesService CreateService(DatabaseContext context)
        {
            return new SourcesService(
                new SourcesRepository(context),
                NullLogger<SourcesService>.Instance);
        }

        #region GetSourceTextForKey

        [Fact]
        public async Task GetSourceTextForKey_NullLanguage_ReturnDefault()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testEn = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                Name = "english",
                Text = "english"
            };
            var testEsp = new Esp()
            {
                Id = 1,
                Key = testEn.Key,
                Name = "spanish",
                Text = "spanish"
            };
            context.SourcesEn.Add(testEn);
            context.SourcesEsp.Add(testEsp);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var source = await service.GetSourceTextForKey(testEn.Key);
            
            // assert
            Assert.Equal(testEn.Text, source);
        }

        [Fact]
        public async Task GetSourceTextForKey_InvalidKey_ReturnNull()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testEn = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                Name = "english",
                Text = "english"
            };
            var testEsp = new Esp()
            {
                Id = 1,
                Key = testEn.Key,
                Name = "spanish",
                Text = "spanish"
            };
            context.SourcesEn.Add(testEn);
            context.SourcesEsp.Add(testEsp);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var source = await service.GetSourceTextForKey(Guid.NewGuid());
            
            // assert
            Assert.Null(source);
        }

        [Fact]
        public async Task GetSourceTextForKey_InvalidLanguage_ThrowException()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testEn = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                Name = "english",
                Text = "english"
            };
            var testEsp = new Esp()
            {
                Id = 1,
                Key = testEn.Key,
                Name = "spanish",
                Text = "spanish"
            };
            context.SourcesEn.Add(testEn);
            context.SourcesEsp.Add(testEsp);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            Task Act() => service.GetSourceTextForKey(testEn.Key, "banana");

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task GetSourceTextForKey_Valid_ReturnSource()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testEn = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                Name = "english",
                Text = "english"
            };
            var testEsp = new Esp()
            {
                Id = 1,
                Key = testEn.Key,
                Name = "spanish",
                Text = "spanish"
            };
            context.SourcesEn.Add(testEn);
            context.SourcesEsp.Add(testEsp);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var source = await service.GetSourceTextForKey(testEn.Key, Languages.SPANISH);
            
            // assert
            Assert.Equal(testEsp.Text, source);
        }

        #endregion

        #region GetSourceForKey
        
        [Fact]
        public async Task GetSourceForKey_EmptyGuidKey_ReturnSource()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Id = 1,
                Key = Guid.Empty,
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEsp.Add(testSource);
            await context.SaveChangesAsync();
            var service = CreateService(context);
        
            //act
            var source = await service.GetSourceForKey(
                testSource.Key, null, Languages.SPANISH);
            
            // assert
            Assert.NotNull(source);
            Assert.Equal(testSource.Id, source.Id);
        }

        [Fact]
        public async Task GetSourceForKey_Valid_ReturnSource()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testEn = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "english",
                Text = "english"
            };
            var testEsp = new Esp()
            {
                Id = 1,
                Key = testEn.Key,
                AdventureId = testEn.AdventureId,
                Name = "spanish",
                Text = "spanish"
            };
            context.SourcesEn.Add(testEn);
            context.SourcesEsp.Add(testEsp);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var source = await service.GetSourceForKey(testEn.Key, testEn.AdventureId, Languages.SPANISH);
            
            // assert
            Assert.NotNull(source);
            Assert.Equal(testEsp.Id, source.Id);
            Assert.Equal(testEsp.Text, source.Text);
        }

        #endregion

        #region AddSource

        [Fact]
        public async Task AddSource_SourceAdded()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var source = new Source()
            {
                Id = 1,
                Text = "source text"
            };
            var service = CreateService(context);
            
            // act
            await service.AddSource(source, Languages.ENGLISH);
            await context.SaveChangesAsync();

            // assert
            Assert.Single(context.SourcesEn);
        }

        #endregion
        
        #region RemoveSource
        
        [Fact]
        public async Task RemoveSource_InvalidSourceId_SourceNotRemoved()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Id = 1,
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Id = 2,
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            await service.RemoveSource(352, Languages.DEFAULT);
            await context.SaveChangesAsync();
            
            // assert
            Assert.Equal(2, context.SourcesEn.Count());
            Assert.Single(context.SourcesEsp);
        }

        [Fact]
        public async Task RemoveSource_ValidSourceId_SourceRemoved()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Id = 1,
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Id = 2,
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            await service.RemoveSource(testSource.Id, Languages.SPANISH);
            await context.SaveChangesAsync();
            
            // assert
            Assert.Equal(2, context.SourcesEn.Count());
            Assert.Empty(context.SourcesEsp);
        }

        #endregion

        #region RemoveScriptFromSources

        [Fact]
        public async Task RemoveScriptFromSources_ScriptRemoved()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source",
                Script = new Script()
                {
                    Id = 1,
                    Name = "test script"
                }
            };
            var testSourceTwo = new Esp()
            {
                Id = 2,
                Key = Guid.NewGuid(),
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            var testSourceThree = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 44,
                Name = "test source",
                Text = "test source",
                Script = testSource.Script
            };
            await context.SourcesEsp.AddRangeAsync(testSource, testSourceTwo);
            await context.SourcesEn.AddRangeAsync(testSourceThree);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            await service.RemoveScriptFromSources(testSource.Script.Id);
            await context.SaveChangesAsync();
            
            // assert
            Assert.Null(context.SourcesEn.First(s => s.Id == testSourceThree.Id).Script);
            Assert.Null(context.SourcesEsp.First(s => s.Id == testSource.Id).Script);
        }

        #endregion
        
        #region GetAllSourceForAdventure

        [Fact]
        public async Task GetAllSourceForAdventure_Valid_AllSourceReturned()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Id = 1,
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Id = 2,
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var source = await service.GetAllSourceForAdventure(testSource.AdventureId.Value, Languages.ENGLISH);
            
            // assert
            Assert.Single(source);
        }

        #endregion

        #region GetAllSourceAllLanguagesForAdventure

        [Fact]
        public async Task GetAllSourceAllLanguagesForAdventure()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Id = 1,
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Id = 2,
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var sources = await service.GetAllSourceAllLanguagesForAdventure(testSource.AdventureId.Value);
            
            // assert
            Assert.Equal(2, sources.Count);
            if (sources[0].Language == Languages.ENGLISH)
                Assert.Equal(Languages.SPANISH, sources[1].Language);
            else
                Assert.Equal(Languages.ENGLISH, sources[0].Language);
        }

        #endregion
        
        #region GetSourceById
        
        [Fact]
        public async Task GetSourceById_InvalidSourceId_ReturnNull()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Id = 1,
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Id = 2,
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var source = await service.GetSourceById(6436, Languages.DEFAULT);
            
            // assert
            Assert.Null(source);
        }

        [Fact]
        public async Task GetSourceId_ValidSourceId_SourceReturned()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Id = 1,
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source en"
            };
            var testSourceEnTwo = new En()
            {
                Id = 2,
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            
            // act
            var source = await service.GetSourceById(testSourceEn.Id, Languages.ENGLISH);
            var sourceEsp = await service.GetSourceById(testSource.Id, Languages.SPANISH);

            // assert
            Assert.NotNull(source);
            Assert.Equal("test source en", testSourceEn.Text);
            Assert.NotNull(sourceEsp);
            Assert.Equal("test source", sourceEsp.Text);
        }

        #endregion
    }
}