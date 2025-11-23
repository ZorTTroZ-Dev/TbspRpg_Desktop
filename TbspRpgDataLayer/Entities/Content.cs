using System;

namespace TbspRpgDataLayer.Entities
{
    public class Content
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public ulong Position { get; set; }
        public Guid SourceKey { get; set; }
        
        public Game Game { get; set; }
    }
}