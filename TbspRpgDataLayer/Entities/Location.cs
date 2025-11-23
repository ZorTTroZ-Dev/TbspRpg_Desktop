using System;
using System.Collections.Generic;

namespace TbspRpgDataLayer.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Initial { get; set; }
        public bool Final { get; set; }
        public Guid SourceKey { get; set; }
        public int AdventureId { get; set; }
        public int? EnterScriptId { get; set; }
        public int? ExitScriptId { get; set; }
        
        public Adventure Adventure { get; set; }
        public ICollection<Route> Routes { get; set; }
        public ICollection<Game> Games { get; set; }
        public Script EnterScript { get; set; }
        public Script ExitScript { get; set; }
        public ICollection<AdventureObject> AdventureObjects { get; set; }
    }
}