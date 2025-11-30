using System;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgDataLayer.Tests.Repositories
{
    public class SourcesRepositoryTests() : InMemoryTest("SourceRepositoryTests")
    {
        #region GetSourceTextForKey

        [Fact]
        public async Task GetSourceTextForKey_Valid_SourceReturned()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new En()
            {
                Key = Guid.NewGuid(),
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEn.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            //act
            var text = await repository.GetSourceTextForKey(testSource.Key);
            
            //assert
            Assert.Equal(testSource.Text, text);
        }

        [Fact]
        public async Task GetSourceTextForKey_InvalidKey_ReturnNone()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new En()
            {
                Key = Guid.NewGuid(),
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEn.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            //act
            var text = await repository.GetSourceTextForKey(Guid.NewGuid());
            
            //assert
            Assert.Null(text);
        }
        
        [Fact]
        public async Task GetSourceTextForKey_ChangeLanguage_SourceReturnedInLanguage()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.NewGuid(),
                Name = "test spanish",
                Text = "in spanish"
            };
            context.SourcesEsp.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            //act
            var text = await repository.GetSourceTextForKey(testSource.Key, Languages.SPANISH);
            
            //assert
            Assert.Equal(testSource.Text, text);
        }
        
        [Fact]
        public async Task GetSourceTextForKey_InvalidLanguage_ThrowException()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new En()
            {
                Key = Guid.NewGuid(),
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEn.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
        
            //act
            Task Act() => repository.GetSourceTextForKey(testSource.Key, "banana");
        
            //assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        #endregion

        #region GetSourceForKey

        [Fact]
        public async Task GetSourceForKey_InvalidAdventureId_ReturnNull()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEsp.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
        
            //act
            var source = await repository.GetSourceForKey(
                testSource.Key, 43, Languages.SPANISH);
            
            // assert
            Assert.Null(source);
        }
        
        [Fact]
        public async Task GetSourceForKey_NullLanguage_ReturnEnglish()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Key = testSource.Key,
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
        
            //act
            var source = await repository.GetSourceForKey(
                testSource.Key, 42, null);
            
            // assert
            Assert.NotNull(source);
            Assert.Equal(testSourceEn.Id, source.Id);
        }
        
        [Fact]
        public async Task GetSourceForKey_InvalidKey_ReturnNull()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEsp.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
        
            //act
            var source = await repository.GetSourceForKey(
                Guid.NewGuid(), 42, Languages.SPANISH);
            
            // assert
            Assert.Null(source);
        }
        
        [Fact]
        public async Task GetSourceForKey_EmptyGuidKey_ReturnSource()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.Empty,
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEsp.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
        
            //act
            var source = await repository.GetSourceForKey(
                testSource.Key, 48, Languages.SPANISH);
            
            // assert
            Assert.NotNull(source);
            Assert.Equal(testSource.Id, source.Id);
        }
        
        [Fact]
        public async Task GetSourceForKey_EmptyGuidKeyEmptyAdventure_ReturnSource()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.Empty,
                AdventureId = null,
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEsp.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
        
            //act
            var source = await repository.GetSourceForKey(
                testSource.Key, null, Languages.SPANISH);
            
            // assert
            Assert.NotNull(source);
            Assert.Equal(testSource.Id, source.Id);
        }
        
        [Fact]
        public async Task GetSourceForKey_Valid_ReturnSource()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEsp.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
        
            //act
            var source = await repository.GetSourceForKey(
                testSource.Key, 42, Languages.SPANISH);
            
            // assert
            Assert.NotNull(source);
            Assert.Equal(testSource.Id, source.Id);
        }
        
        [Fact]
        public async Task GetSourceForKey_InValidLanguage_ThrowException()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new En()
            {
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            context.SourcesEn.Add(testSource);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
        
            //act
            Task Act() => repository.GetSourceForKey(
                testSource.Key, testSource.AdventureId,"banana");
        
            //assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        #endregion

        #region AddSource

        [Fact]
        public async Task AddSource_NoLanguage_AddEnglish()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var source = new Source()
            {
                Text = "default"
            };
            var repository = new SourcesRepository(context);
            
            // act
            await repository.AddSource(source, null);
            await context.SaveChangesAsync();

            // assert
            Assert.Single(context.SourcesEn);
        }

        [Fact]
        public async Task AddSource_English_AddEnglish()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var source = new Source()
            {
                Text = "default"
            };
            var repository = new SourcesRepository(context);
            
            // act
            await repository.AddSource(source, Languages.ENGLISH);
            await context.SaveChangesAsync();

            // assert
            Assert.Single(context.SourcesEn);
        }

        [Fact]
        public async Task AddSource_BadLanguage_ThrowException()
        {
            // arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var source = new Source()
            {
                Text = "default"
            };
            var repository = new SourcesRepository(context);
            
            // act
            Task Act() => repository.AddSource(source, "banana");
        
            //assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
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
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            await repository.RemoveSource(674, Languages.DEFAULT);
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
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Key = Guid.NewGuid(),
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            await repository.RemoveSource(testSource.Id, Languages.SPANISH);
            await context.SaveChangesAsync();
            
            // assert
            Assert.Empty(context.SourcesEsp);
            Assert.Equal(2, context.SourcesEn.Count());
        }

        #endregion
        
        #region RemoveAllSourceForAdventure

        [Fact]
        public async Task RemoveAllSourceForAdventure_AllSourceRemoved()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            await repository.RemoveAllSourceForAdventure(testSource.AdventureId.Value);
            await context.SaveChangesAsync();
            
            // assert
            Assert.Empty(context.SourcesEsp);
            Assert.Single(context.SourcesEn);
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
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            var source = await repository.GetAllSourceForAdventure(testSource.AdventureId.Value, 
                Languages.ENGLISH);
            
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
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            var sources = await repository.GetAllSourceAllLanguagesForAdventure(testSource.AdventureId.Value);
            
            // assert
            Assert.Equal(2, sources.Count);
            if (sources[0].Language == Languages.ENGLISH)
                Assert.Equal(Languages.SPANISH, sources[1].Language);
            else
                Assert.Equal(Languages.ENGLISH, sources[0].Language);
        }

        #endregion

        #region GetSourcesWithScript

        [Fact]
        public async Task GetSourcesWithScript_HasSources_ReturnSources()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source",
                Script = new Script()
                {
                    Name = "test script"
                }
            };
            var testSourceTwo = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            await context.SourcesEsp.AddRangeAsync(testSource, testSourceTwo);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            var sources = await repository.GetSourcesWithScript(testSource.Script.Id);
            
            // assert
            Assert.Single(sources);
        }

        [Fact]
        public async Task GetSourcesWithScript_MultipleLanguages_ReturnSources()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source",
                Script = new Script()
                {
                    Name = "test script"
                }
            };
            var testSourceTwo = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            var testSourceThree = new En()
            {
                Key = Guid.NewGuid(),
                AdventureId = 44,
                Name = "test source",
                Text = "test source",
                Script = testSource.Script
            };
            await context.SourcesEsp.AddRangeAsync(testSource, testSourceTwo);
            await context.SourcesEn.AddRangeAsync(testSourceThree);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            var sources = await repository.GetSourcesWithScript(testSource.Script.Id);
            
            // assert
            Assert.Equal(2, sources.Count);
            Assert.NotNull(sources.FirstOrDefault(source => source.Id == testSourceThree.Id));
            Assert.NotNull(sources.FirstOrDefault(source => source.Id == testSource.Id));
        }

        [Fact]
        public async Task GetSourcesWithScript_NoSources_ReturnEmpty()
        {
            //arrange
            await using var context = new DatabaseContext(DbContextOptions);
            var testSource = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source",
                Script = new Script()
                {
                    Name = "test script"
                }
            };
            var testSourceTwo = new Esp()
            {
                Key = Guid.NewGuid(),
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            var testSourceThree = new En()
            {
                Key = Guid.NewGuid(),
                AdventureId = 44,
                Name = "test source",
                Text = "test source",
                Script = testSource.Script
            };
            await context.SourcesEsp.AddRangeAsync(testSource, testSourceTwo);
            await context.SourcesEn.AddRangeAsync(testSourceThree);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            var sources = await repository.GetSourcesWithScript(744);
            
            // assert
            Assert.Empty(sources);
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
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            var source = await repository.GetSourceById(658, Languages.SPANISH);
            
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
                Key = Guid.NewGuid(),
                AdventureId = 42,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEn = new En()
            {
                Key = testSource.Key,
                AdventureId = testSource.AdventureId,
                Name = "test source",
                Text = "test source"
            };
            var testSourceEnTwo = new En()
            {
                Key = testSource.Key,
                AdventureId = 43,
                Name = "test source two",
                Text = "test source two"
            };
            context.SourcesEsp.Add(testSource);
            context.SourcesEn.Add(testSourceEn);
            context.SourcesEn.Add(testSourceEnTwo);
            await context.SaveChangesAsync();
            var repository = new SourcesRepository(context);
            
            // act
            var source = await repository.GetSourceById(testSource.Id, Languages.SPANISH);

            // assert
            Assert.NotNull(source);
            Assert.Equal(testSource.Id, source.Id);
        }

        #endregion
    }
}