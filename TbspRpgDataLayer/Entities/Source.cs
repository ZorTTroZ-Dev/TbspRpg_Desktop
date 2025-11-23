using System;

namespace TbspRpgDataLayer.Entities
{
    public class Source
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public int? AdventureId { get; set; }
        public int? ScriptId { get; set; }
        public string Language { get; set; }
        
        public Script Script { get; set; }
    }
}