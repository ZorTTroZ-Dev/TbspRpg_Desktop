using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services;

public interface ILanguagesService : IBaseService
{
    Task<Language> GetLanguagesById(int languageId);
    Task<List<Language>> GetAllLanguagesAsync();
    List<Language> GetAllLanguages();
    Language GetDefaultLanguage();
    void Seed();
}

public class LanguagesService: ILanguagesService
{
    private readonly ILanguagesRepository _languagesRepository;
    private readonly ILogger<LanguagesService> _logger;
    
    public LanguagesService(ILanguagesRepository languagesRepository,
        ILogger<LanguagesService> logger)
    {
        _languagesRepository = languagesRepository;
        _logger = logger;
    }
    
    public async Task SaveChanges()
    {
        await _languagesRepository.SaveChanges();
    }

    public Task<Language> GetLanguagesById(int languageId)
    {
        return _languagesRepository.GetLanguagesById(languageId);
    }

    public Task<List<Language>> GetAllLanguagesAsync()
    {
        return _languagesRepository.GetAllLanguagesAsync();
    }

    public List<Language> GetAllLanguages()
    {
        return _languagesRepository.GetAllLanguages();
    }

    public Language GetDefaultLanguage()
    {
        return _languagesRepository.GetDefaultLanguage();
    }

    public void Seed()
    {
        _languagesRepository.Seed();
    }
}