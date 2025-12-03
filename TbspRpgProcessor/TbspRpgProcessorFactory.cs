using System;
using System.Data;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer;
using TbspRpgSettings;

namespace TbspRpgProcessor;

public class TbspRpgProcessorFactory
{
    private static TbspRpgProcessor _instance;
    public static void Create(TbspRpgDataServiceFactory dataServiceFactory, ILoggerFactory loggerFactory)
    {
        _instance = new TbspRpgProcessor(
            dataServiceFactory.SourcesService,
            dataServiceFactory.ScriptsService,
            dataServiceFactory.AdventuresService,
            dataServiceFactory.RoutesService,
            dataServiceFactory.LocationsService,
            dataServiceFactory.GamesService,
            dataServiceFactory.ContentsService,
            dataServiceFactory.AdventureObjectService,
            dataServiceFactory.AdventureObjectSourceService,
            dataServiceFactory.CopyService,
            new TbspRpgUtilities(),
            loggerFactory.CreateLogger<TbspRpgProcessor>()
        );
    }
    
    public static TbspRpgProcessor TbspRpgProcessor()
    {
        return _instance ?? throw new NullReferenceException("couldn't load TbspRpgProcessor, did you call create");
    }
}