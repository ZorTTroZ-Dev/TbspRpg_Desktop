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
            Assert.Equal("banana_script", scripts[0].Name);
        }

         #endregion
        
    }
}