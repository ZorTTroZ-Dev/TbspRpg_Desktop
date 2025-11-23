using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.ArgumentModels;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services
{
    public interface IContentsService : IBaseService
    {
        Task AddContent(Content content);
        Task<Content> GetContentForGameAtPosition(int gameId, ulong position);
        Task<List<Content>> GetAllContentForGame(int gameId);
        Task<Content> GetLatestForGame(int gameId);
        Task<List<Content>> GetContentForGameAfterPosition(int gameId, ulong position);
        Task<List<Content>> GetPartialContentForGame(int gameId, ContentFilterRequest filterRequest);
        void RemoveContents(IEnumerable<Content> contents);
        Task RemoveAllContentsForGame(int gameId);
        Task<bool> DoesAdventureContentUseSource(int adventureId, Guid sourceKey);
        Task<List<Content>> GetAdventureContentsWithSource(int adventureId, Guid sourceKey);
    }
    
    public class ContentsService : IContentsService
    {
        private readonly IContentsRepository _contentsRepository;
        private readonly ILogger<ContentsService> _logger;

        public ContentsService(IContentsRepository contentsRepository,
            ILogger<ContentsService> logger)
        {
            _contentsRepository = contentsRepository;
            _logger = logger;
        }
        
        public async Task SaveChanges()
        {
            await _contentsRepository.SaveChanges();
        }

        public async Task AddContent(Content content)
        {
            var dbContent = await GetContentForGameAtPosition(content.GameId, content.Position);
            if(dbContent == null)
                await _contentsRepository.AddContent(content);
        }

        public Task<Content> GetContentForGameAtPosition(int gameId, ulong position)
        {
            return _contentsRepository.GetContentForGameAtPosition(gameId, position);
        }

        public Task<List<Content>> GetAllContentForGame(int gameId)
        {
            return _contentsRepository.GetContentForGame(gameId);
        }

        public async Task<Content> GetLatestForGame(int gameId)
        {
            var contents = await _contentsRepository.GetContentForGameReverse(gameId, null, 1);
            return contents.FirstOrDefault();
        }

        public Task<List<Content>> GetContentForGameAfterPosition(int gameId, ulong position)
        {
            return _contentsRepository.GetContentForGameAfterPosition(gameId, position);
        }

        public async Task<List<Content>> GetPartialContentForGame(int gameId, ContentFilterRequest filterRequest)
        {
            List<Content> contents = null;
            if (string.IsNullOrEmpty(filterRequest.Direction) || filterRequest.IsForward())
            {
                contents = await _contentsRepository.GetContentForGame(
                    gameId,
                    (int?) filterRequest.Start,
                    (int?) filterRequest.Count);
            } 
            else if (filterRequest.IsBackward())
            {
                contents = await _contentsRepository.GetContentForGameReverse(
                    gameId,
                    (int?) filterRequest.Start,
                    (int?) filterRequest.Count);
            }
            else
            {
                //we can't parse the direction
                throw new ArgumentException($"invalid direction argument {filterRequest.Direction}");
            }

            return contents;
        }

        public void RemoveContents(IEnumerable<Content> contents)
        {
            _contentsRepository.RemoveContents(contents);
        }

        public async Task RemoveAllContentsForGame(int gameId)
        {
            await _contentsRepository.RemoveAllContentsForGame(gameId);
        }

        public async Task<bool> DoesAdventureContentUseSource(int adventureId, Guid sourceKey)
        {
            var contents = await GetAdventureContentsWithSource(adventureId, sourceKey);
            return contents.Any();
        }

        public Task<List<Content>> GetAdventureContentsWithSource(int adventureId, Guid sourceKey)
        {
            return _contentsRepository.GetAdventureContentsWithSource(adventureId, sourceKey);
        }
    }
}