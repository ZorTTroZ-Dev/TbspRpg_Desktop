using TbspRpgDataLayer.Entities;

namespace TbspRpgProcessor.Entities;

public class CopyCreateModel
{
    public Copy Copy { get; set; }
    public bool Save { get; set; } = true;
}