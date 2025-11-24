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
    public class LocationProcessorTests : ProcessorTest
    {
        #region UpdateLocation

        [Fact]
        public async Task UpdateLocation_BadLocationId_ThrowException()
        {
            // arrange
            var testLocation = new Location()
            {
                Id = 1,
                Name = "test location",
                Initial = true,
                SourceKey = Guid.NewGuid()
            };
            var testSource = new En()
            {
                Id = 1,
                Key = testLocation.SourceKey,
                Name = "test location",
                Text = "test source"
            };
            var locations = new List<Location>() { testLocation };
            var sources = new List<En>() {testSource};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Locations = locations,
                Sources = sources
            });
            
            // act
            Task Act() => processor.UpdateLocation(new LocationUpdateModel() {
                Location = new Location() {
                    Id = 7,
                    Name = "updated location name",
                    Initial = false,
                    SourceKey = testLocation.SourceKey
                },
                Source = new En()
                {
                    Id = testSource.Id,
                    Key = testSource.Key,
                    Name = "test location",
                    Text = "updated source"
                },
                Language = Languages.ENGLISH
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task UpdateLocation_NewSource_NewSourceCreated()
        {
            // arrange
            var testLocation = new Location()
            {
                Id = 1,
                Name = "test location",
                Initial = true,
                SourceKey = Guid.Empty
            };
            var locations = new List<Location>() { testLocation };
            var sources = new List<En>();
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Locations = locations,
                Sources = sources
            });
            
            // act
            await processor.UpdateLocation(new LocationUpdateModel() {
                Location = new Location()
                {
                    Id = testLocation.Id,
                    Name = "updated location name",
                    Initial = true,
                    AdventureObjects = new List<AdventureObject>()
                },
                Source = new En()
                {
                    Key = Guid.Empty,
                    Text = "updated source"
                },
                Language = Languages.ENGLISH
            });

            // assert
            Assert.Single(sources);
            Assert.Single(locations);
            Assert.Equal("updated location name", sources[0].Name);
            Assert.Equal("updated source", sources[0].Text);
            Assert.Equal(sources[0].Key, locations[0].SourceKey);
        }

        [Fact]
        public async Task UpdateLocation_BadSourceId_ThrowException()
        {
            // arrange
            var testLocation = new Location()
            {
                Id = 1,
                Name = "test location",
                Initial = true,
                SourceKey = Guid.NewGuid()
            };
            var testSource = new En()
            {
                Id = 1,
                Key = testLocation.SourceKey,
                Name = "test location",
                Text = "test source"
            };
            var locations = new List<Location>() { testLocation };
            var sources = new List<En>() {testSource};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Locations = locations,
                Sources = sources
            });
            
            // act
            Task Act() => processor.UpdateLocation(new LocationUpdateModel() {
                Location = new Location()
                {
                    Id = testLocation.Id,
                    Name = "updated location name",
                    Initial = false,
                    SourceKey = testLocation.SourceKey,
                    AdventureObjects = new List<AdventureObject>()
                },
                Source = new En()
                {
                    Id = testSource.Id,
                    Key = Guid.NewGuid(),
                    Name = "test location",
                    Text = "updated source"
                },
                Language = Languages.ENGLISH
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }
        
        [Fact]
        public async Task UpdateLocation_UpdateLocationAndSource_LocationSourceUpdated()
        {
            // arrange
            var testLocation = new Location()
            {
                Id = 1,
                Name = "test location",
                Initial = true,
                AdventureId = 1,
                SourceKey = Guid.NewGuid()
            };
            var testSource = new En()
            {
                Id = 1,
                Key = testLocation.SourceKey,
                AdventureId = 1,
                Name = "test location",
                Text = "test source"
            };
            var locations = new List<Location>() { testLocation };
            var sources = new List<En>() {testSource};
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Locations = locations,
                Sources = sources
            });
            
            // act
            await processor.UpdateLocation(new LocationUpdateModel() {
                Location = new Location()
                {
                    Id = testLocation.Id,
                    Name = "updated location name",
                    Initial = false,
                    SourceKey = testLocation.SourceKey,
                    AdventureId = testLocation.AdventureId,
                    AdventureObjects = new List<AdventureObject>()
                },
                Source = new En()
                {
                    Id = testSource.Id,
                    Key = testLocation.SourceKey,
                    AdventureId = testSource.AdventureId,
                    Name = "test location",
                    Text = "updated source"
                },
                Language = Languages.ENGLISH
            });
            
            // assert
            Assert.Single(sources);
            Assert.Single(locations);
            Assert.False(locations[0].Initial);
            Assert.Equal("updated location name", locations[0].Name);
            Assert.Equal("updated source", sources[0].Text);
        }

        [Fact]
        public async Task UpdateLocation_EmptyLocationId_CreateNewLocation()
        {
            // arrange
            var adventureObject = new AdventureObject()
            {
                Id = 1,
                Name = "test_object"
            };
            var adventureObjects = new List<AdventureObject>() {adventureObject};
            var locations = new List<Location>();
            var sources = new List<En>();
            var processor = CreateTbspRpgProcessor(new TestTbspRpgProcessorData() {
                Locations = locations,
                Sources = sources,
                AdventureObjects = adventureObjects
            });
            
            // act
            await processor.UpdateLocation(new LocationUpdateModel() {
                Location = new Location()
                {
                    Name = "new location name",
                    Initial = false,
                    SourceKey = Guid.Empty,
                    AdventureObjects = new List<AdventureObject>()
                    {
                        adventureObject
                    }
                },
                Source = new En()
                {
                    Key = Guid.Empty,
                    Name = "new location name",
                    Text = "updated source"
                },
                Language = Languages.ENGLISH
            });
            
            // assert
            Assert.Single(sources);
            Assert.Single(locations);
            Assert.False(locations[0].Initial);
            Assert.Equal("new location name", locations[0].Name);
            Assert.Equal("updated source", sources[0].Text);
        }

        #endregion

        #region RemoveLocation

        [Fact]
        public async Task RemoveLocation_Valid_LocationRemoved()
        {
            // arrange
            var locationId = 1;
            var testRoute = new Route()
            {
                Id = 1,
                Name = "test route",
                LocationId = locationId
            };
            var testLocation = new Location()
            {
                Id = locationId,
                Name = "test location",
                Initial = true,
                SourceKey = Guid.NewGuid(),
                Routes = new List<Route>()
                {
                    testRoute
                }
            };
            var testSource = new En()
            {
                Id = 1,
                Key = testLocation.SourceKey,
                Name = "test location",
                Text = "test source"
            };
            var locations = new List<Location>() { testLocation };
            var sources = new List<En>() {testSource};
            var routes = new List<Route>() {testRoute};
            var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
                Routes = routes,
                Locations = locations,
                Sources = sources
            });
            
            // act
            await processor.RemoveLocation(new LocationRemoveModel()
            {
                LocationId = locationId
            });
            
            // assert
            Assert.Empty(locations);
            Assert.Empty(routes);
        }

        [Fact]
        public async Task RemoveLocation_InvalidLocationId_ExceptionThrown()
        {
            // arrange
            var locationId = 1;
            var testRoute = new Route()
            {
                Id = 1,
                Name = "test route",
                LocationId = locationId
            };
            var testLocation = new Location()
            {
                Id = locationId,
                Name = "test location",
                Initial = true,
                SourceKey = Guid.NewGuid(),
                Routes = new List<Route>()
                {
                    testRoute
                }
            };
            var testSource = new En()
            {
                Id = 1,
                Key = testLocation.SourceKey,
                Name = "test location",
                Text = "test source"
            };
            var locations = new List<Location>() { testLocation };
            var sources = new List<En>() {testSource};
            var routes = new List<Route>() {testRoute};
            var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
                Routes = routes,
                Locations = locations,
                Sources = sources
            });
            
            // act
            Task Act() => processor.RemoveLocation(new LocationRemoveModel()
            {
                LocationId = 16
            });

            // assert
            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        #endregion

        #region RemoveLocations

        [Fact]
        public async Task RemoveLocations_LocationsRemoved()
        {
            // arrange
            var locationId = 1;
            var locationIdTwo = 2;
            var testRoute = new Route()
            {
                Id = 1,
                Name = "test route",
                LocationId = locationId
            };
            var testRouteTwo = new Route()
            {
                Id = 2,
                Name = "test route two",
                LocationId = locationId
            };
            var testLocation = new Location()
            {
                Id = locationId,
                Name = "test location",
                Initial = true,
                SourceKey = Guid.NewGuid(),
                Routes = new List<Route>()
                {
                    testRoute
                }
            };
            var testLocationTwo = new Location()
            {
                Id = locationIdTwo,
                Name = "test location two",
                Initial = true,
                SourceKey = Guid.NewGuid(),
                Routes = new List<Route>()
                {
                    testRouteTwo
                }
            };
            var testSource = new En()
            {
                Id = 1,
                Key = testLocation.SourceKey,
                Name = "test location",
                Text = "test source"
            };
            var locations = new List<Location>() { testLocation, testLocationTwo };
            var sources = new List<En>() { testSource };
            var routes = new List<Route>() { testRoute, testRouteTwo };
            var processor = CreateTbspRpgProcessor( new TestTbspRpgProcessorData() {
                Routes = routes,
                Locations = locations,
                Sources = sources
            });
            
            // act
            await processor.RemoveLocations(new LocationsRemoveModel() {
                Locations = new List<Location>()
                {
                    testLocation, testLocationTwo
                }
            });
            
            // assert
            Assert.Empty(locations);
            Assert.Empty(routes);
        }

        #endregion
    }
}