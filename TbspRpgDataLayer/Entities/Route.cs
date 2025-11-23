using System;

namespace TbspRpgDataLayer.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid SourceKey { get; set; }
        public Guid RouteTakenSourceKey { get; set; }
        public int LocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public int? RouteTakenScriptId { get; set; }
        
        public Location Location { get; set; }
        public Location DestinationLocation { get; set; }
        public Script RouteTakenScript { get; set; }
    }
}