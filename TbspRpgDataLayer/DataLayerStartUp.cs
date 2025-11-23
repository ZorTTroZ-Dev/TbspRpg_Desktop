using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TbspRpgDataLayer.Repositories;
using TbspRpgDataLayer.Services;

namespace TbspRpgDataLayer
{
    public static class DataLayerStartUp
    {
        public static void InitializeDataLayer(IServiceCollection services)
        {
            // services and repositories
            services.AddScoped<ISourcesRepository, SourcesRepository>();
            services.AddScoped<IRoutesRepository, RoutesRepository>();
            services.AddScoped<ILocationsRepository, LocationsRepository>();
            services.AddScoped<IAdventuresRepository, AdventuresRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IContentsRepository, ContentsRepository>();
            services.AddScoped<IScriptsRepository, ScriptsRepository>();
            services.AddScoped<IAdventureObjectRepository, AdventureObjectRepository>();
            services.AddScoped<IAdventureObjectGameStateRepository, AdventureObjectGameStateRepository>();
            services.AddScoped<IAdventureObjectSourceRepository, AdventureObjectSourceRepository>();
            
            services.AddScoped<IAdventuresService, AdventuresService>();
            services.AddScoped<IGamesService, GamesService>();
            services.AddScoped<ILocationsService, LocationsService>();
            services.AddScoped<IContentsService, ContentsService>();
            services.AddScoped<IRoutesService, RoutesService>();
            services.AddScoped<ISourcesService, SourcesService>();
            services.AddScoped<IScriptsService, ScriptsService>();
            services.AddScoped<IAdventureObjectService, AdventureObjectService>();
            services.AddScoped<IAdventureObjectGameStateService, AdventureObjectGameStateService>();
            services.AddScoped<IAdventureObjectSourceService, AdventureObjectSourceService>();
            
            // setup the database connection
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.AddDbContext<DatabaseContext>(
                options => options.UseSqlite(connectionString)
            );
        }
    }
}