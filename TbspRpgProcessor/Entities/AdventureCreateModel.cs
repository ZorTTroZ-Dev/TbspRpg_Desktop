using System.Collections.Generic;
using TbspRpgDataLayer.Entities;

namespace TbspRpgProcessor.Entities;

public class AdventureCreateModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Language> Languages { get; set; }
    public Language DescriptionLanguage { get; set; }
}