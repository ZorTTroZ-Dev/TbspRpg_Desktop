using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;
using TbspRpgSettings.Settings;

namespace TbspRpgDataLayer.Services
{
    public interface ISourcesService: IBaseService
    {
        Task<string> GetSourceTextForKey(Guid key, string language = null);
        Task<Source> GetSourceForKey(Guid key, int? adventureId, string language);
        Task AddSource(Source source, string language = null);
        Task RemoveAllSourceForAdventure(int adventureId);
        Task RemoveSource(int sourceId, string language);
        Task RemoveScriptFromSources(int scriptId);
        Task<List<Source>> GetAllSourceForAdventure(int adventureId, string language);
        Task<List<Source>> GetAllSourceAllLanguagesForAdventure(int adventureId);
        Task<Source> GetSourceById(int sourceId, string language);
        void Seed();
    }
    
    public class SourcesService : ISourcesService
    {
        private readonly ISourcesRepository _sourcesRepository;
        private readonly ILogger<SourcesService> _logger;

        public SourcesService(ISourcesRepository sourcesRepository,
            ILogger<SourcesService> logger)
        {
            _logger = logger;
            _sourcesRepository = sourcesRepository;
        }

        public Task<string> GetSourceTextForKey(Guid key, string language = null)
        {
            language ??= Languages.DEFAULT;
            return _sourcesRepository.GetSourceTextForKey(key, language);
        }

        public Task<Source> GetSourceForKey(Guid key, int? adventureId, string language)
        {
            return _sourcesRepository.GetSourceForKey(key, adventureId, language);
        }
        
        public async Task AddSource(Source source, string language = null)
        {
            await _sourcesRepository.AddSource(source, language);
            _logger.LogInformation("Source {@Source} added of {language}", source, language);
        }

        public async Task RemoveAllSourceForAdventure(int adventureId)
        {
            await _sourcesRepository.RemoveAllSourceForAdventure(adventureId);
        }

        public async Task RemoveSource(int sourceId, string language)
        {
            await _sourcesRepository.RemoveSource(sourceId, language);
        }

        public async Task RemoveScriptFromSources(int scriptId)
        {
            var sources = await _sourcesRepository.GetSourcesWithScript(scriptId);
            foreach (var source in sources)
            {
                source.ScriptId = null;
            }
        }

        public Task<List<Source>> GetAllSourceForAdventure(int adventureId, string language)
        {
            return _sourcesRepository.GetAllSourceForAdventure(adventureId, language);
        }

        public Task<List<Source>> GetAllSourceAllLanguagesForAdventure(int adventureId)
        {
            return _sourcesRepository.GetAllSourceAllLanguagesForAdventure(adventureId);
        }

        public Task<Source> GetSourceById(int sourceId, string language)
        {
            return _sourcesRepository.GetSourceById(sourceId, language);
        }

        public void Seed()
        {
            _sourcesRepository.Seed();
        }

        public async Task SaveChanges()
        {
            await _sourcesRepository.SaveChanges();
        }
    }
}