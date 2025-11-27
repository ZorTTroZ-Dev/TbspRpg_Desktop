using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;

namespace TbspRpgDataLayer;

public class TbspRpgDataLayer
{
    private readonly DatabaseContext _dbContext;
    public readonly AdventuresService AdventuresService;
    public readonly SourcesService SourcesService;
    
    private TbspRpgDataLayer(string connectionString, ILoggerFactory loggerFactory, bool initialize = false)
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
    }
    ~TbspRpgDataLayer()
    {
        _dbContext?.Dispose();
    }
    
    private static TbspRpgDataLayer _instance;
    public static void Connect(string connectionString, ILoggerFactory loggerFactory, bool initialize = false)
    {
        _instance ??= new TbspRpgDataLayer(connectionString, loggerFactory, initialize);
    }
    public static TbspRpgDataLayer Load()
    {
        return _instance ?? throw new ConstraintException("connect needs to be called before loading data layer");
    }
}