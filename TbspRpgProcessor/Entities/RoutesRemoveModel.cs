using System;
using System.Collections.Generic;

namespace TbspRpgProcessor.Entities;

public class RoutesRemoveModel
{
    public ICollection<int> CurrentRouteIds { get; set; }
    public int LocationId { get; set; }
}