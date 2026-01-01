using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services;

public interface ICopyService : IBaseService
{
    Task<string> GetCopyTextForKey(Guid key, Language language);
    Task<List<Copy>> GetCopyForKey(Guid key);
    Task<Copy> GetCopy(Guid key, Language language);
    Task<List<Copy>> GetCopyForLanguage(Language language);
    void Seed(List<Language> languages);
    Task AddCopy(Copy copy);
}

public class CopyService: ICopyService
{
    private readonly ICopyRepository _copyRepository;
    private readonly ILogger<CopyService> _logger;
    
    public CopyService(ICopyRepository copyRepository,
        ILogger<CopyService> logger)
    {
        _copyRepository = copyRepository;
        _logger = logger;
    }
    
    public async Task SaveChanges()
    {
        await _copyRepository.SaveChanges();
    }

    public Task<string> GetCopyTextForKey(Guid key, Language language)
    {
        return _copyRepository.GetCopyTextForKey(key, language);
    }

    public Task<List<Copy>> GetCopyForKey(Guid key)
    {
        return _copyRepository.GetCopyForKey(key);
    }

    public Task<Copy> GetCopy(Guid key, Language language)
    {
        return _copyRepository.GetCopy(key, language);
    }

    public Task<List<Copy>> GetCopyForLanguage(Language language)
    {
        return _copyRepository.GetCopyForLanguage(language);
    }

    public void Seed(List<Language> languages)
    {
        _copyRepository.Seed(languages);
    }

    public async Task AddCopy(Copy copy)
    {
        await _copyRepository.AddCopy(copy);
    }
}