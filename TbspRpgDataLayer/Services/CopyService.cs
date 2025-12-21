using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TbspRpgDataLayer.Entities;
using TbspRpgDataLayer.Repositories;

namespace TbspRpgDataLayer.Services;

public interface ICopyService : IBaseService
{
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

    public void Seed(List<Language> languages)
    {
        _copyRepository.Seed(languages);
    }

    public async Task AddCopy(Copy copy)
    {
        await _copyRepository.AddCopy(copy);
    }
}