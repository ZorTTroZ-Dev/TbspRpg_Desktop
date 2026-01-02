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
    public class SourceProcessorTests : ProcessorTest
    {
        #region CreateOrUpdateSource

        [Fact]
        public async Task CreateOrUpdateSource_BadSourceId_ThrowException()
        {
            // arrange
            var testEn = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 1
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Sources = new List<En>() {testEn}
            });
            
            // act
            Task Act() => processor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
                Source = new Source()
                {
                    Id = testEn.Id,
                    AdventureId = testEn.AdventureId,
                    Key = Guid.NewGuid()
                },
                Language = Languages.ENGLISH
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task CreateOrUpdateSource_EmptyKey_CreateNewSource()
        {
            // arrange
            var testEn = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 1
            };
            var sources = new List<En>()
            {
                testEn
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Sources = sources
            });
            
            
            // act
            var dbSource = await processor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
                Source = new Source()
                {
                    AdventureId = testEn.AdventureId,
                    Key = Guid.Empty,
                    Text = "new source"
                }, 
                Language = Languages.ENGLISH
            });
            
            // assert
            Assert.NotNull(dbSource);
            Assert.Equal(2, sources.Count);
            Assert.Equal("new source", dbSource.Text);
            Assert.NotEqual(Guid.Empty, dbSource.Key);
        }

        [Fact]
        public async Task CreateOrUpdateSource_ExistingSource_UpdateKey()
        {
            // arrange
            var testEn = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 1,
                Text = "original text"
            };
            var sources = new List<En>()
            {
                testEn
            };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Sources = new List<En>() {testEn}
            });
            
            // act
            var dbSource = await processor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
                Source = new Source()
                {
                    Id = testEn.Id,
                    Key = testEn.Key,
                    AdventureId = testEn.AdventureId,
                    Text = "updated text"
                },
                Language = Languages.ENGLISH
            });
            
            // assert
            Assert.Single(sources);
            Assert.Equal("updated text", dbSource.Text);
        }

        [Fact]
        public async Task CreateOrUpdateSource_NewWithScript_ScriptCompiled()
        {
            // arrange
            var sources = new List<En>();
            var scripts = new List<Script>();
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Sources = sources,
                Scripts = scripts
            });
            
            // act
            var dbSource = await processor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
                Source = new Source()
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
                },
                Language = Languages.ENGLISH
            });
            
            // assert
            Assert.Single(sources);
            Assert.Single(scripts);
            Assert.Equal("banana_script", scripts[0].Name);
        }

        [Fact]
        public async Task CreateOrUpdateSource_ExistingWithScript_ScriptUpdated()
        {
            // arrange
            var testSource = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 1,
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
            testSource.ScriptId = testScript.Id;
            var sources = new List<En>() { testSource };
            var scripts = new List<Script>() { testScript };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Sources = sources,
                Scripts = scripts
            });
            
            // act
            var dbSource = await processor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
                Source = new Source()
                {
                    Id = testSource.Id,
                    Key = testSource.Key,
                    AdventureId = testSource.AdventureId,
                    Name = "banana",
                    Text = @"This is a text with some *emphasis*, {script:
                    if(game:GetGameStatePropertyBoolean('boolean'))
                    then
                        return 'spain'
                    end
                }.",
                    ScriptId = testSource.ScriptId
                },
                Language = Languages.ENGLISH
            });
            
            // assert
            Assert.Single(sources);
            Assert.Single(scripts);
            Assert.Contains("spain", scripts[0].Content);
        }

        [Fact]
        public async Task CreateOrUpdateSource_ExistingNoScript_ScriptRemoved()
        {
            // arrange
            var testSource = new En()
            {
                Id = 1,
                Key = Guid.NewGuid(),
                AdventureId = 1,
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
            testSource.ScriptId = testScript.Id;
            var sources = new List<En>() { testSource };
            var scripts = new List<Script>() { testScript };
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Sources = sources,
                Scripts = scripts
            });
            
            // act
            var dbSource = await processor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
                Source = new Source()
                {
                    Id = testSource.Id,
                    Key = testSource.Key,
                    AdventureId = testSource.AdventureId,
                    Name = "banana",
                    Text = @"This is a text with some *emphasis*.",
                    ScriptId = testSource.ScriptId
                },
                Language = Languages.ENGLISH
            });
            
            // assert
            Assert.Single(sources);
            Assert.Empty(scripts);
            Assert.Null(sources[0].Script);
        }

         [Fact]
         public async Task CreateOrUpdateSource_ExistingAddScript_ScriptCompiled()
         {
             // arrange
             var testSource = new En()
             {
                 Id = 1,
                 Key = Guid.NewGuid(),
                 AdventureId = 1,
                 Name = "banana",
                 Text = @"This is a text with some *emphasis*."
             };
             var sources = new List<En>() { testSource };
             var scripts = new List<Script>();
             var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                 Sources = sources,
                 Scripts = scripts
             });
             
             // act
             var dbSource = await processor.CreateOrUpdateSource(new SourceCreateOrUpdateModel() {
                 Source = new Source()
                 {
                     Id = testSource.Id,
                     Key = testSource.Key,
                     AdventureId = testSource.AdventureId,
                     Name = "banana",
                     Text = @"This is a text with some *emphasis*, {script:
                     if(game:GetGameStatePropertyBoolean('boolean'))
                     then
                         return 'spain'
                     end
                 }."
                 },
                 Language = Languages.ENGLISH
             });
             
             // assert
             Assert.Single(sources);
             Assert.Single(scripts);
             Assert.Contains("spain", scripts[0].Content);
         }

         #endregion

         #region GetSourceForKey

         [Fact]
         public async Task GetSourceForKey_DoesntExist_ReturnNull()
         {
             // arrange
             var testEn = new En()
             {
                 Id = 1,
                 Key = Guid.NewGuid(),
                 AdventureId = 1,
                 Text = "original text"
             };
             var sources = new List<En>()
             {
                 testEn
             };
             var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                 Sources = sources
             });
             
             // act
             var source = await processor.GetSourceForKey(new SourceForKeyModel()
             {
                 Key = testEn.Key,
                 AdventureId = 17,
                 Language = Languages.ENGLISH
             });
             
             // assert
             Assert.Null(source);
         }

         [Fact]
         public async Task GetSourceForKey_ValidNotProcessed_SourceTextNotHtml()
         {
             // arrange
             var testEn = new En()
             {
                 Id = 1,
                 Key = Guid.NewGuid(),
                 AdventureId = 1,
                 Text = "This is a text with some *emphasis*"
             };
             var sources = new List<En>()
             {
                 testEn
             };
             var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                 Sources = sources
             });
             
             // act
             var source = await processor.GetSourceForKey(new SourceForKeyModel()
             {
                 Key = testEn.Key,
                 AdventureId = testEn.AdventureId.Value,
                 Language = Languages.ENGLISH,
                 Processed = false
             });
             
             // Assert
             Assert.Equal("This is a text with some *emphasis*", source.Text);
         }
         
         [Fact]
         public async Task GetSourceForKey_ContainsScript_SourceTextScriptResolved()
         {
             // arrange
             var testGame = new Game()
             {
                 Id = 1,
                 GameState = "{\"number\": 42, \"string\": \"banana\", \"boolean\": false}"
             };
             
             var testEn = new En()
             {
                 Id = 1,
                 Name = "test_source",
                 Key = Guid.NewGuid(),
                 AdventureId = 1,
                 Text = @"This is a text with some *emphasis*, {script:
                     if(game:GetGameStatePropertyBoolean('boolean'))
                     then
                         return 'banana'
                     else return 'france' end
                 }, {script:
                     if(game:GetGameStatePropertyBoolean('boolean'))
                     then
                         return 'france'
                     else return 'banana' end
                 }."
             };
             var sources = new List<En>()
             {
                 testEn
             };
             var scripts = new List<Script>();
             var games = new List<Game>() {testGame};
             var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                 Sources = sources,
                 Scripts = scripts,
                 Games = games
             });
             
             // act
             var source = await processor.GetSourceForKey(new SourceForKeyModel()
             {
                 Key = testEn.Key,
                 AdventureId = testEn.AdventureId.Value,
                 Language = Languages.ENGLISH,
                 Processed = true,
                 Game = testGame
             });
             
             // Assert
             Assert.Single(scripts);
             Assert.Equal("This is a text with some *emphasis*, france, banana.", source.Text);
         }
         
         [Fact]
         public async Task GetSourceForKey_ContainsScript_InvalidScriptId_ExceptionThrown()
         {
             // arrange
             var testGame = new Game()
             {
                 Id = 1,
                 GameState = "{\"number\": 42, \"string\": \"banana\", \"boolean\": false}"
             };

             var testEn = new En()
             {
                 Id = 1,
                 Name = "test_source",
                 Key = Guid.NewGuid(),
                 AdventureId = 1,
                 Text = @"This is a text with some *emphasis*, {script:
                     if(game:GetGameStatePropertyBoolean('boolean'))
                     then
                         return 'banana'
                     else return 'france' end
                 }, {script:
                     if(game:GetGameStatePropertyBoolean('boolean'))
                     then
                         return 'france'
                     else return 'banana' end
                 }.",
                 ScriptId = 34
             };
             var sources = new List<En>()
             {
                 testEn
             };
             var scripts = new List<Script>();
             var games = new List<Game>() {testGame};
             var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                 Sources = sources,
                 Scripts = scripts,
                 Games = games
             });
             
             // act
             Task Act() => processor.GetSourceForKey(new SourceForKeyModel()
             {
                 Key = testEn.Key,
                 AdventureId = testEn.AdventureId.Value,
                 Language = Languages.ENGLISH,
                 Processed = true,
                 Game = testGame
             });
             
             // Assert
             await Assert.ThrowsAsync<Exception>(Act);
         }
         
        [Fact]
        public async Task GetSourceForKey_ContainsScript_InvalidScript_ExceptionThrown()
        {
            // arrange
            var testGame = new Game()
            {
                Id = 1,
                GameState = "{\"number\": 42, \"string\": \"banana\", \"boolean\": false}"
            };

            var script = new Script()
            {
                Id = 1,
                Name = "viva_la_france",
                Type = ScriptTypes.LuaScript,
                Content = @"function func0()
if(game:GetGameStatePropertyBoolean('boolean'))
                    then
                        return 'banana'
                    else return 'france' end
end
function func1()
if(game:GetGameStatePropertyBoolean('boolean'))
                    then
                        return 'france'
                    else return 'banana' end
end
function run()
	result0 = func0()
	result1 = func1()
	
	result = result0 .. result1
end"
            };

            var testEn = new En()
            {
                Id = 1,
                Name = "test_source",
                Key = Guid.NewGuid(),
                AdventureId = 1,
                Text = @"This is a text with some *emphasis*, {script:
                    if(game:GetGameStatePropertyBoolean('boolean'))
                    then
                        return 'banana'
                    else return 'france' end
                }, {script:
                    if(game:GetGameStatePropertyBoolean('boolean'))
                    then
                        return 'france'
                    else return 'banana' end
                }.",
                ScriptId = script.Id
            };
            script.AdventureId = testEn.AdventureId.Value;
            var sources = new List<En>() {testEn};
            var scripts = new List<Script>() {script};
            var games = new List<Game>() {testGame};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Sources = sources,
                Scripts = scripts,
                Games = games
            });
            
            // act
            Task Act() => processor.GetSourceForKey(new SourceForKeyModel()
            {
                Key = testEn.Key,
                AdventureId = testEn.AdventureId.Value,
                Language = Languages.ENGLISH,
                Processed = true,
                Game = testGame
            });
            
            // Assert
            await Assert.ThrowsAsync<Exception>(Act);
        }
        
         [Fact]
         public async Task GetSourceForKey_ContainsObject_SourceTextScriptResolved()
         {
             // arrange
             var testGame = new Game()
             {
                 Id = 1,
                 GameState = "{\"number\": 42, \"string\": \"banana\", \"boolean\": false}"
             };

             var objectNameSourceEn = new En()
             {
                 Id = 1,
                 Key = Guid.NewGuid(),
                 Name = "name_source",
                 Text = "object_name"
             };
             var objectDescriptionSourceEn = new En()
             {
                 Id = 2,
                 Key = Guid.NewGuid(),
                 Name = "description_source",
                 Text = "object_description"
             };
             var obj = new AdventureObject()
             {
                 Id = 1,
                 Name = "test_object",
                 Description = "test_description",
                 NameSourceKey = objectNameSourceEn.Key,
                 DescriptionSourceKey = objectDescriptionSourceEn.Key
             };
             var testEn = new En()
             {
                 Id = 3,
                 Name = "test_source",
                 Key = Guid.NewGuid(),
                 AdventureId = 1,
                 Text = @"This is a text with some *emphasis*, {object:" + obj.Id + "}."
             };
             var sources = new List<En>() {testEn, objectDescriptionSourceEn, objectNameSourceEn};
             var scripts = new List<Script>();
             var games = new List<Game>() {testGame};
             var objects = new List<AdventureObject>() {obj};
             var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                 Sources = sources,
                 Scripts = scripts,
                 Games = games,
                 AdventureObjects = objects
             });
             
             // act
             var source = await processor.GetSourceForKey(new SourceForKeyModel()
             {
                 Key = testEn.Key,
                 AdventureId = testEn.AdventureId.Value,
                 Language = Languages.ENGLISH,
                 Processed = true,
                 Game = testGame
             });
             
             // Assert
             Assert.Equal("This is a text with some *emphasis*, <object>{\"tooltip\":\"object_description\",\"text\":\"object_name\"}<object>.", source.Text);
         }

         #endregion

         #region GetUnreferencedSource

         [Fact]
         public async Task GetUnreferencedSource_OneNotUsed_SourceReturned()
         {
             // arrange
             var testAdventure = new Adventure()
             {
                 Id = 1,
                 InitialCopyKey = Guid.NewGuid()
             };
             var testLocation = new Location()
             {
                 Id = 1,
                 Adventure = testAdventure,
                 AdventureId = testAdventure.Id,
                 SourceKey = Guid.NewGuid()
             };
             var testRoute = new Route()
             {
                 Id = 1,
                 Location = testLocation,
                 LocationId = testLocation.Id,
                 SourceKey = Guid.NewGuid()
             };
             var testGame = new Game()
             {
                 Id = 1,
                 Adventure = testAdventure,
                 AdventureId = testAdventure.Id
             };
             var testContent = new Content()
             {
                 Game = testGame,
                 GameId = testGame.Id,
                 Id = 1,
                 SourceKey = Guid.NewGuid()
             };
             var testScript = new Script()
             {
                 Id = 1,
                 AdventureId = testAdventure.Id,
                 Adventure = testAdventure,
                 Content = Guid.NewGuid().ToString()
             };
             var testSources = new List<En>()
             {
                 new()
                 {
                     Id = 1,
                     Key = testAdventure.InitialCopyKey,
                     AdventureId = testAdventure.Id
                 },
                 new()
                 {
                     Id = 2,
                     Key = testLocation.SourceKey,
                     AdventureId = testAdventure.Id
                 },
                 new()
                 {
                     Id = 3,
                     Key = testRoute.SourceKey,
                     AdventureId = testAdventure.Id
                 },
                 new()
                 {
                     Id = 4,
                     Key = testContent.SourceKey,
                     AdventureId = testAdventure.Id
                 },
                 new()
                 {
                     Id = 5,
                     Key = Guid.Parse(testScript.Content),
                     AdventureId = testAdventure.Id
                 },
                 new() // not referenced or used
                 {
                     Id = 6,
                     Key = Guid.NewGuid(),
                     AdventureId = testAdventure.Id,
                     Text = "unreferenced"
                 }
             };
             var testAdventures = new List<Adventure>() {testAdventure};
             var testLocations = new List<Location>() {testLocation};
             var testRoutes = new List<Route>() {testRoute};
             var testGames = new List<Game>() {testGame};
             var testContents = new List<Content>() {testContent};
             var testScripts = new List<Script>() {testScript};
             var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
                 Scripts = testScripts,
                 Adventures = testAdventures,
                 Routes = testRoutes,
                 Locations = testLocations,
                 Sources = testSources,
                 Games = testGames,
                 Contents = testContents
             });
             
             // act
             var unreferencedSources = await processor.GetUnreferencedSources(new UnreferencedSourceModel()
             {
                 AdventureId = testAdventure.Id
             });
             
             // assert
             Assert.Single(unreferencedSources);
             Assert.Equal("unreferenced", unreferencedSources[0].Text);
         }

         #endregion
         
         #region RemoveSource

         [Fact]
         public async Task RemoveSource_ValidSourceId_SourceRemoved()
         {
             // arrange
             var testSources = new List<En>()
             {
                 new()
                 {
                     Id = 1,
                     Language = Languages.ENGLISH
                 }
             };
             var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                 Sources = testSources
             });
             
             // act
             await processor.RemoveSource(new SourceRemoveModel()
             {
                 SourceId = testSources[0].Id
             });
             
             // assert
             Assert.Empty(testSources);
         }

        #endregion
    }
}