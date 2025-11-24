using System;
using TbspRpgDataLayer.Entities;

namespace TbspRpgProcessor.Entities
{
    public class AdventureUpdateModel
    {
        public Adventure Adventure { get; set; }
        public Source InitialSource { get; set; }
        public Source DescriptionSource { get; set; }
        public Guid UserId { get; set; }
        public string Language { get; set; }
    }
}