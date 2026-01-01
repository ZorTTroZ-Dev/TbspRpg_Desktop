using System.Collections.Generic;

namespace TbspRpgDataLayer.Entities;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public ICollection<Adventure> Adventures { get; set; }
    public ICollection<Copy> Copy { get; set; }
}