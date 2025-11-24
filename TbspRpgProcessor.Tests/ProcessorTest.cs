using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Tests;
using TbspRpgProcessor.Entities;
using TbspRpgSettings;

namespace TbspRpgProcessor.Tests
{
    public class TestTbspRpgProcessorData
    {
        public ICollection<Script> Scripts { get; set; }
        public ICollection<Adventure> Adventures { get; set; }
        public ICollection<Route> Routes { get; set; }
        public ICollection<Location> Locations { get; set; }
        public ICollection<En> Sources { get; set; }
        public ICollection<Game> Games { get; set; }
        public ICollection<Content> Contents { get; set; }
        public ICollection<AdventureObject> AdventureObjects { get; set; }
    }
    
    public class ProcessorTest
    {
        public static ITbspRpgProcessor CreateTbspRpgProcessor(TestTbspRpgProcessorData data)
        {
            data.Adventures ??= new List<Adventure>();
            data.Routes ??= new List<Route>();
            data.Locations ??= new List<Location>();
            data.Sources ??= new List<En>();
            data.Games ??= new List<Game>();
            data.Contents ??= new List<Content>();
            data.Scripts ??= new List<Script>();
            data.AdventureObjects ??= new List<AdventureObject>();
            
            var scriptsService = MockServices.MockDataLayerScriptsService(data.Scripts);
            var adventuresService = MockServices.MockDataLayerAdventuresService(data.Adventures);
            var routesService = MockServices.MockDataLayerRoutesService(data.Routes);
            var locationsService = MockServices.MockDataLayerLocationsService(data.Locations);
            var sourcesService = MockServices.MockDataLayerSourcesService(data.Sources);
            var gamesService = MockServices.MockDataLayerGamesService(data.Games);
            var contentsService = MockServices.MockDataLayerContentsService(data.Contents);
            var adventureObjectsService = MockServices.MockDataLayerAdventureObjectsService(data.AdventureObjects);
            var adventureObjectSourceService =
                MockServices.MockDataLayerAdventureObjectsSourceService(data.AdventureObjects, data.Sources);
            
            return new TbspRpgProcessor(
                sourcesService,
                scriptsService,
                adventuresService,
                routesService,
                locationsService,
                gamesService,
                contentsService,
                adventureObjectsService,
                adventureObjectSourceService,
                new TbspRpgUtilities(),
                NullLogger<TbspRpgProcessor>.Instance);
        }

        public static ITbspRpgProcessor MockTbspRpgProcessor(string exceptionEmail, int exceptionId)
        {
            var tbspProcessor = new Mock<ITbspRpgProcessor>();
            
            tbspProcessor.Setup(service =>
                    service.ExecuteScript(It.IsAny<ScriptExecuteModel>()))
                .Callback((ScriptExecuteModel scriptExecuteModel) =>
                {
                    if (scriptExecuteModel.ScriptId == exceptionId)
                    {
                        throw new ArgumentException("invalid script id");
                    }
                });
            
            tbspProcessor.Setup(service =>
                    service.UpdateScript(It.IsAny<ScriptUpdateModel>()))
                .Callback((ScriptUpdateModel scriptUpdateModel) =>
                {
                    if (scriptUpdateModel.script.Id == exceptionId)
                    {
                        throw new ArgumentException("invalid script id");
                    }
                });

            tbspProcessor.Setup(service =>
                    service.RemoveScript(It.IsAny<ScriptRemoveModel>()))
                .Callback((ScriptRemoveModel scriptRemoveModel) =>
                {
                    if (scriptRemoveModel.ScriptId == exceptionId)
                    {
                        throw new ArgumentException("invalid script id");
                    }
                });
            
            tbspProcessor.Setup(service =>
                    service.UpdateRoute(It.IsAny<RouteUpdateModel>()))
                .Callback((RouteUpdateModel routeUpdateModel) =>
                {
                    if (routeUpdateModel.route.Id == exceptionId)
                        throw new ArgumentException("can't update route");
                });

            tbspProcessor.Setup(service =>
                    service.RemoveRoutes(It.IsAny <RoutesRemoveModel>()))
                .Callback((RoutesRemoveModel routesRemoveModel) => { });
            
            tbspProcessor.Setup(service =>
                    service.RemoveRoute(It.IsAny<RouteRemoveModel>()))
                .Callback((RouteRemoveModel routeRemoveModel) =>
                {
                    if (routeRemoveModel.RouteId == exceptionId)
                    {
                        throw new ArgumentException("invalid route id");
                    }
                });
            
            tbspProcessor.Setup(service =>
                    service.ChangeLocationViaRoute(It.IsAny<MapChangeLocationModel>()))
                .Callback((MapChangeLocationModel mapChangeLocationModel) =>
                {
                    if (mapChangeLocationModel.GameId == exceptionId)
                    {
                        throw new ArgumentException("can't change location");
                    }
                });
            
            tbspProcessor.Setup(service =>
                    service.UpdateLocation(It.IsAny<LocationUpdateModel>()))
                .Callback((LocationUpdateModel locationUpdateModel) =>
                {
                    if (locationUpdateModel.Location.Id == exceptionId)
                    {
                        throw new ArgumentException("can't update location");
                    }
                });
            
            tbspProcessor.Setup(service =>
                    service.RemoveLocation(It.IsAny<LocationRemoveModel>()))
                .Callback((LocationRemoveModel locationRemoveModel) =>
                {
                    if (locationRemoveModel.LocationId == exceptionId)
                    {
                        throw new ArgumentException("invalid location id");
                    }
                });
            
            tbspProcessor.Setup(service =>
                    service.StartGame(It.IsAny<GameStartModel>()))
                .ReturnsAsync((GameStartModel gameStartModel) =>
                {
                    return new Game()
                    {
                        Id = 12
                    };
                });
            
            tbspProcessor.Setup(service =>
                    service.RemoveGame(It.IsAny<GameRemoveModel>()))
                .Callback((GameRemoveModel gameRemoveModel) =>
                {
                    if (gameRemoveModel.GameId == exceptionId)
                    {
                        throw new ArgumentException("can't remove game");
                    }
                });
            
            tbspProcessor.Setup(service =>
                    service.UpdateAdventure(It.IsAny<AdventureUpdateModel>()))
                .Callback((AdventureUpdateModel adventureUpdateModel) =>
                {
                    if (adventureUpdateModel.Adventure.Id == exceptionId)
                        throw new ArgumentException("invalid adventure id");
                });
            
            tbspProcessor.Setup(service =>
                    service.RemoveAdventure(It.IsAny<AdventureRemoveModel>()))
                .Callback((AdventureRemoveModel adventureRemoveModel) =>
                {
                    if (adventureRemoveModel.AdventureId == exceptionId)
                        throw new ArgumentException("invalid adventure id");
                });
            
            tbspProcessor.Setup(service =>
                    service.RemoveAdventureObject(It.IsAny<AdventureObjectRemoveModel>()))
                .Callback((AdventureObjectRemoveModel adventureRemoveModel) =>
                {
                    if (adventureRemoveModel.AdventureObjectId == exceptionId)
                        throw new ArgumentException("invalid adventure object id");
                });
            
            tbspProcessor.Setup(service =>
                    service.UpdateAdventureObject(It.IsAny<AdventureObjectUpdateModel>()))
                .Callback((AdventureObjectUpdateModel adventureObjectUpdateModel) =>
                {
                    if (adventureObjectUpdateModel.AdventureObject.Id == exceptionId)
                    {
                        throw new ArgumentException("invalid adventure object id");
                    }
                });

            return tbspProcessor.Object;
        }
    }
}