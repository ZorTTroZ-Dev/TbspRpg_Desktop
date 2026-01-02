using System;
using System.Collections.Generic;

namespace TbspRpgDataLayer.Entities
{
    public class Adventure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid InitialCopyKey { get; set; }
        public Guid DescriptionCopyKey { get; set; }
        public int? InitializationScriptId { get; set; }
        public int? TerminationScriptId { get; set; }
        
        public ICollection<Location> Locations { get; set; }
        public ICollection<Game> Games { get; set; }
        public ICollection<Script> Scripts { get; set; }
        public Script InitializationScript { get; set; }
        public Script TerminationScript { get; set; }
        public ICollection<Language> Languages { get; set; }
    }
}