using System;

namespace TbspRpgDataLayer.ArgumentModels
{
    public class RouteFilter
    {
        public int? LocationId { get; set; }
        public int? DestinationLocationId { get; set; }
        public int? AdventureId { get; set; }
    }
}