using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor.Entities;
using TbspRpgSettings.Settings;
using Xunit;

namespace TbspRpgProcessor.Tests.Processors
{
    public class CopyProcessorTests : ProcessorTest
    {
        #region CreateCopy

        [Fact]
        public async Task CreateCopy_EmptyKey_NewCopyKeyGenerated()
        {
            // arrange
            var copy = new List<Copy>
            {
                new()
                {
                    AdventureId = 1,
                    Id = 1,
                    Key = Guid.NewGuid()
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Copy = copy
            });
            
            
            // act
            var dbCopy = await processor.CreateCopy(new CopyCreateModel()
            {
                Copy = new Copy()
                {
                    AdventureId = copy[0].AdventureId,
                    Key = Guid.Empty,
                    Text = "new copy"
                }
            });
            
            // assert
            Assert.NotNull(dbCopy);
            Assert.Equal(2, copy.Count);
            Assert.Equal("new copy", dbCopy.Text);
            Assert.NotEqual(Guid.Empty, dbCopy.Key);
        }
        
        [Fact]
        public async Task CreateCopy_WithLanguages_MultipleCopyGenerated()
        {
            // arrange
            var copy = new List<Copy>
            {
                new()
                {
                    AdventureId = 1,
                    Id = 1,
                    Key = Guid.NewGuid()
                }
            };
            var languages = new List<Language>
            {
                new()
                {
                    Id = 1,
                    Code = "en",
                    Name = "English"
                },
                new()
                {
                    Id = 2,
                    Code = "es",
                    Name = "Spanish"
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Copy = copy
            });
            
            
            // act
            var dbCopy = await processor.CreateCopy(new CopyCreateModel()
            {
                Copy = new Copy()
                {
                    AdventureId = copy[0].AdventureId,
                    Key = Guid.Empty,
                    Text = "new copy",
                    Language = languages[0]
                },
                Languages = languages
            });
            
            // assert
            Assert.NotNull(dbCopy);
            Assert.Equal(3, copy.Count);
            Assert.Equal("new copy", dbCopy.Text);
            Assert.NotEqual(Guid.Empty, dbCopy.Key);
            Assert.Equal(1, dbCopy.Language.Id);
            Assert.Equal("", copy.First(cpy => cpy.Key  == dbCopy.Key && cpy.Language.Id == 2).Text);
        }
        
        [Fact]
        public async Task CreateCopy_PopulatedKey_NewCopy()
        {
            // arrange
            var copy = new List<Copy>
            {
                new()
                {
                    AdventureId = 1,
                    Id = 1,
                    Key = Guid.NewGuid()
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Copy = copy
            });
            
            
            // act
            var guid = Guid.NewGuid();
            var dbCopy = await processor.CreateCopy(new CopyCreateModel()
            {
                Copy = new Copy()
                {
                    AdventureId = copy[0].AdventureId,
                    Key = guid,
                    Text = "new copy"
                }
            });
            
            // assert
            Assert.NotNull(dbCopy);
            Assert.Equal(2, copy.Count);
            Assert.Equal("new copy", dbCopy.Text);
            Assert.Equal(guid, dbCopy.Key);
        }

        [Fact]
        public async Task CreateCopy_NewWithScript_ScriptCompiled()
        {
            // arrange
            var copy = new List<Copy>();
            var scripts = new List<Script>();
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Copy = copy,
                Scripts = scripts
            });
            
            // act
            var dbCopy = await processor.CreateCopy(new CopyCreateModel() {
                Copy = new Copy()
                {
                    Id = 1,
                    Key = Guid.Empty,
                    AdventureId = 1,
                    Name = "banana",
                    Text = @"This is a text with some *emphasis*, {script:
                    if(game:GetGameStatePropertyBoolean('boolean'))
                    then
                        return 'banana'
                    else return 'france' end
                }."
                }
            });
            
            // assert
            Assert.Single(copy);
            Assert.Single(scripts);
            Assert.Equal("banana_Script", scripts[0].Name);
        }

        #endregion

        #region UpdateCopy
        
        [Fact]
        public async Task UpdateCopy_BadCopyId_ThrowException()
        {
            // arrange
            var copy = new List<Copy>()
            {
                new()
                {
                    Id = 1,
                    Key = Guid.NewGuid(),
                    AdventureId = 1,
                    Language = new Language
                    {
                        Id = 1,
                        Code = "en"
                    }
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Copy = copy
            });
            
            // act
            Task Act() => processor.UpdateCopy(new CopyUpdateModel() {
                Copy = new Copy()
                {
                    Id = copy[0].Id,
                    AdventureId = copy[0].AdventureId,
                    Key = Guid.NewGuid(),
                    Language = new Language
                    {
                        Id = 1,
                        Code = "en"
                    } 
                }
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }
        
        [Fact]
        public async Task UpdateCopy_ExistingCopy_UpdateText()
        {
            // arrange
            var copy = new List<Copy>()
            {
                new()
                {
                    Id = 1,
                    Key = Guid.NewGuid(),
                    AdventureId = 1,
                    Text = "original text",
                    Language = new Language
                    {
                        Id = 1,
                        Code = "en"
                    }
                }
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Copy = copy
            });
            
            // act
            var dbCopy = await processor.UpdateCopy(new CopyUpdateModel() {
                Copy = new Copy()
                {
                    Id = copy[0].Id,
                    Key = copy[0].Key,
                    AdventureId = copy[0].AdventureId,
                    Language = copy[0].Language,
                    Text = "updated text"
                }
            });
            
            // assert
            Assert.Single(copy);
            Assert.Equal("updated text", dbCopy.Text);
        }
        
        [Fact]
        public async Task UpdateCopy_ExistingWithScript_ScriptUpdated()
        {
            // arrange
            var testCopy = new Copy()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 1,
                Language = new Language
                {
                    Id = 1,
                    Code = "en"
                },
                Name = "banana",
                Text = @"This is a text with some *emphasis*, {script:
                    if(game:GetGameStatePropertyBoolean('boolean'))
                    then
                        return 'banana'
                    else return 'france' end
                }."
            };
            var testScript = new Script()
            {
                Id = 1,
                AdventureId = 1,
                Type = ScriptTypes.LuaScript,
                Name = "banana_script",
                Content = @"function func0()
                    if(game:GetGameStatePropertyBoolean('boolean'))
                                        then
                                            return 'banana'
                                        else return 'france' end
                    end
                    function run()
	                    result0 = func0()
	                    
	                    result = result0
                    end"
            };
            testCopy.ScriptId = testScript.Id;
            var copy = new List<Copy>() { testCopy };
            var scripts = new List<Script>() { testScript };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Copy = copy,
                Scripts = scripts
            });
            
            // act
            var dbCopy = await processor.UpdateCopy(new CopyUpdateModel() {
                Copy = new Copy()
                {
                    Id = copy[0].Id,
                    Key = copy[0].Key,
                    AdventureId = copy[0].AdventureId,
                    Language = copy[0].Language,
                    Name = "banana",
                    Text = @"This is a text with some *emphasis*, {script:
                    if(game:GetGameStatePropertyBoolean('boolean'))
                    then
                        return 'spain'
                    end
                }.",
                    ScriptId = copy[0].ScriptId
                }
            });
            
            // assert
            Assert.Single(copy);
            Assert.Single(scripts);
            Assert.Contains("spain", scripts[0].Content);
        }

        [Fact]
        public async Task UpdateCopy_ExistingNoScript_ScriptRemoved()
        {
            // arrange
            var testCopy = new Copy()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 1,
                Language = new Language
                {
                    Id = 1,
                    Code = "en"
                },
                Name = "banana",
                Text = @"This is a text with some *emphasis*, <script:
                    if(game:GetGameStatePropertyBoolean('boolean'))
                    then
                        return 'banana'
                    else return 'france' end
                >."
            };
            var testScript = new Script()
            {
                Id = 1,
                AdventureId = 1,
                Type = ScriptTypes.LuaScript,
                Name = "banana_script",
                Content = @"function func0()
                    if(game:GetGameStatePropertyBoolean('boolean'))
                                        then
                                            return 'banana'
                                        else return 'france' end
                    end
                    function run()
	                    result0 = func0()
	                    
	                    result = result0
                    end"
            };
            testCopy.ScriptId = testScript.Id;
            var copy = new List<Copy>() { testCopy };
            var scripts = new List<Script>() { testScript };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Copy = copy,
                Scripts = scripts
            });
            
            // act
            var dbCopy = await processor.UpdateCopy(new CopyUpdateModel() {
                Copy = new Copy()
                {
                    Id = testCopy.Id,
                    Key = testCopy.Key,
                    AdventureId = testCopy.AdventureId,
                    Language = testCopy.Language,
                    Name = "banana",
                    Text = @"This is a text with some *emphasis*.",
                    ScriptId = testCopy.ScriptId
                }
            });
            
            // assert
            Assert.Single(copy);
            Assert.Empty(scripts);
            Assert.Null(copy[0].Script);
        }

         [Fact]
         public async Task UpdateCopy_ExistingAddScript_ScriptCompiled()
         {
             // arrange
             var testCopy = new Copy()
             {
                 Id = 1,
                 Key = Guid.NewGuid(),
                 AdventureId = 1,
                 Language = new Language
                 {
                     Id = 1,
                     Code = "en"
                 },
                 Name = "banana",
                 Text = @"This is a text with some *emphasis*."
             };
             var copy = new List<Copy>() { testCopy };
             var scripts = new List<Script>();
             var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                 Copy = copy,
                 Scripts = scripts
             });
             
             // act
             var dbCopy = await processor.UpdateCopy(new CopyUpdateModel() {
                 Copy = new Copy()
                 {
                     Id = testCopy.Id,
                     Key = testCopy.Key,
                     AdventureId = testCopy.AdventureId,
                     Language = testCopy.Language,
                     Name = "banana",
                     Text = @"This is a text with some *emphasis*, {script:
                     if(game:GetGameStatePropertyBoolean('boolean'))
                     then
                         return 'spain'
                     end
                 }."
                 }
             });
             
             // assert
             Assert.Single(copy);
             Assert.Single(scripts);
             Assert.Contains("spain", scripts[0].Content);
         }
        
        #endregion
    }
}