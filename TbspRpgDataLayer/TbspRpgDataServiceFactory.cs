using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;

namespace TbspRpgDataLayer;

public class TbspRpgDataServiceFactory
{
    private readonly DatabaseContext _dbContext;
    
    // Services
    public readonly IAdventuresService AdventuresService;
    public readonly ISourcesService SourcesService;
    public readonly IScriptsService ScriptsService;
    public readonly IRoutesService RoutesService;
    public readonly ILocationsService LocationsService;
    public readonly IGamesService GamesService;
    public readonly IAdventureObjectService AdventureObjectService;
    public readonly IContentsService ContentsService;
    public readonly IAdventureObjectSourceService AdventureObjectSourceService;
    
    private TbspRpgDataServiceFactory(string connectionString, ILoggerFactory loggerFactory, bool initialize = false)
    {
        var contextFactory = new DatabaseContextFactory();
        _dbContext = contextFactory.CreateDbContext(connectionString);

        if (initialize)
        {
            _dbContext.Database.Migrate();
        }
        
        // create services
        var sourcesRepository = new SourcesRepository(_dbContext);
        SourcesService = new SourcesService(sourcesRepository, loggerFactory.CreateLogger<SourcesService>());
        if (initialize)
            SourcesService.Seed();
        var adventuresRepository = new AdventuresRepository(_dbContext);
        AdventuresService = new AdventuresService(adventuresRepository,
            loggerFactory.CreateLogger<AdventuresService>());
        var scriptsRepository = new ScriptsRepository(_dbContext);
        ScriptsService = new ScriptsService(scriptsRepository, loggerFactory.CreateLogger<ScriptsService>());
        var routesRepository = new RoutesRepository(_dbContext);
        RoutesService = new RoutesService(routesRepository, loggerFactory.CreateLogger<RoutesService>());
        var locationsRepository = new LocationsRepository(_dbContext);
        LocationsService = new LocationsService(locationsRepository, loggerFactory.CreateLogger<LocationsService>());
        var gamesRepository = new GameRepository(_dbContext);
        GamesService = new GamesService(gamesRepository, loggerFactory.CreateLogger<GamesService>());
        var adventureObjectRepository = new AdventureObjectRepository(_dbContext);
        AdventureObjectService = new AdventureObjectService(adventureObjectRepository,
            loggerFactory.CreateLogger<AdventureObjectService>());
        var contentsRepository = new ContentsRepository(_dbContext);
        ContentsService = new ContentsService(contentsRepository, loggerFactory.CreateLogger<ContentsService>());
        var adventureObjectSourceRepository = new AdventureObjectSourceRepository(_dbContext);
        AdventureObjectSourceService = new AdventureObjectSourceService(adventureObjectSourceRepository,
            loggerFactory.CreateLogger<AdventureObjectSourceService>());
    }
    
    ~TbspRpgDataServiceFactory()
    {
        _dbContext?.Dispose();
    }
    
    private static TbspRpgDataServiceFactory _instance;
    public static void Connect(string connectionString, ILoggerFactory loggerFactory, bool initialize = false)
    {
        _instance ??= new TbspRpgDataServiceFactory(connectionString, loggerFactory, initialize);
    }
    public static TbspRpgDataServiceFactory Load()
    {
        return _instance ?? throw new ConstraintException("connect needs to be called before loading data layer");
    }
}