using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;
using TbspRpgSettings.Settings;

namespace TbspRpgDataLayer.Repositories
{
    public interface ISourcesRepository: IBaseRepository
    {
        Task<string> GetSourceTextForKey(Guid key, string language = null);
        Task<Source> GetSourceForKey(Guid key, int? adventureId, string language);
        Task AddSource(Source source, string language);
        Task RemoveAllSourceForAdventure(int adventureId);
        Task RemoveSource(int sourceId, string language);
        Task<List<Source>> GetAllSourceForAdventure(int adventureId, string language);
        Task<List<Source>> GetAllSourceAllLanguagesForAdventure(int adventureId);
        Task<List<Source>> GetSourcesWithScript(int scriptId);
        Task<Source> GetSourceById(int sourceId, string language);
        void Seed();
    }
    
    public class SourcesRepository : ISourcesRepository
    {
        private readonly DatabaseContext _databaseContext;

        public SourcesRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        private IQueryable<Source> GetQueryRoot(string language)
        {
            IQueryable<Source> query = null;
            if (language == null || language == Languages.ENGLISH)
            {
                query = _databaseContext.SourcesEn.AsQueryable();
            }
            if (language == Languages.SPANISH)
            {
                query = _databaseContext.SourcesEsp.AsQueryable();
            }
            return query;
        }

        public Task<string> GetSourceTextForKey(Guid key, string language = null)
        {
            var query = GetQueryRoot(language);
            if (query != null)
                return query.Where(s => s.Key == key)
                    .Select(s => s.Text)
                    .FirstOrDefaultAsync();
            
            throw new ArgumentException($"invalid language {language}");
        }

        public Task<Source> GetSourceForKey(Guid key, int? adventureId, string language)
        {
            var query = GetQueryRoot(language);
            if(query != null && key != Guid.Empty)
                return query.FirstOrDefaultAsync(source => 
                    source.Key == key && source.AdventureId == adventureId);
            if(query != null && key == Guid.Empty) 
                return query.FirstOrDefaultAsync(source => 
                    source.Key == key);
            
            throw new ArgumentException($"invalid language {language}");
        }

        private En SourceToEn(Source source)
        {
            return new En()
            {
                Id = source.Id,
                Key = source.Key,
                AdventureId = source.AdventureId,
                Name = source.Name,
                Text = source.Text
            };
        }

        private Esp SourcetoEsp(Source source)
        {
            return new Esp()
            {
                Id = source.Id,
                Key = source.Key,
                AdventureId = source.AdventureId,
                Name = source.Name,
                Text = source.Text
            };
        }

        public async Task AddSource(Source source, string language)
        {
            if (language == Languages.ENGLISH || language == null)
            {
                await _databaseContext.SourcesEn.AddAsync(SourceToEn(source));
                return;
            }

            if (language == Languages.SPANISH)
            {
                await _databaseContext.SourcesEsp.AddAsync(SourcetoEsp(source));
                return;
            }

            throw new ArgumentException($"invalid language {language}");
        }

        public async Task RemoveAllSourceForAdventure(int adventureId)
        {
            // have to remove all source for each language
            foreach (var language in Languages.GetAllLanguages())
            {
                var source = await GetAllSourceForAdventure(adventureId, language);
                _databaseContext.RemoveRange(source);
            }
        }

        public async Task RemoveSource(int sourceId, string language)
        {
            var query = GetQueryRoot(language);
            var source = await query.FirstOrDefaultAsync(source => source.Id == sourceId);
            if (source != null)
                _databaseContext.Remove(source);
        }

        public async Task<List<Source>> GetAllSourceForAdventure(int adventureId, string language)
        {
            var query = GetQueryRoot(language);
            var sources = await query.Where(source => source.AdventureId == adventureId).ToListAsync();
            sources.ForEach(source => source.Language = language);
            return sources;
        }

        public async Task<List<Source>> GetAllSourceAllLanguagesForAdventure(int adventureId)
        {
            List<Source> sources = new List<Source>();
            foreach (var language in Languages.GetAllLanguages())
            {
                var languageSources = await GetQueryRoot(language)
                    .Where(source => source.AdventureId == adventureId)
                    .ToListAsync();
                languageSources.ForEach(source => source.Language = language);
                sources.AddRange(languageSources);
            }
            return sources;
        }

        public async Task<List<Source>> GetSourcesWithScript(int scriptId)
        {
            List<Source> sources = new List<Source>();
            foreach (var language in Languages.GetAllLanguages())
            {
                var query = GetQueryRoot(language);
                sources.AddRange(await query.Where(source => source.ScriptId == scriptId).ToListAsync());
            }
            return sources;
        }

        public async Task<Source> GetSourceById(int sourceId, string language)
        {
            var query = GetQueryRoot(language);
            var source = await query.FirstOrDefaultAsync(source => source.Id == sourceId);
            return source;
        }

        public void Seed()
        {
            foreach (var language in Languages.GetAllLanguages())
            {
                var query = GetQueryRoot(language);
                var emptySource =  query.FirstOrDefault(source => source.Key == Guid.Empty);
                if (emptySource == null)
                {
                    var source = new Source()
                    {
                        Key = Guid.Empty,
                        AdventureId = null
                    };
                    if (language == Languages.ENGLISH || language == null)
                    {
                        source.Text = "Empty Source";
                        _databaseContext.SourcesEn.Add(SourceToEn(source));
                    }
                    if (language == Languages.SPANISH)
                    {
                        source.Text = "Fuente Vacia";
                        _databaseContext.SourcesEsp.Add(SourcetoEsp(source));
                    }
                }
            }
            _databaseContext.SaveChanges();
        }

        public async Task SaveChanges()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}