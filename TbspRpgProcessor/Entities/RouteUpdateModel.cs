using TbspRpgDataLayer.Entities;

namespace TbspRpgProcessor.Entities
{
    public class RouteUpdateModel
    {
        public Route route { get; set; }
        public Source source { get; set; }
        public Source successSource { get; set; }
        public string newDestinationLocationName { get; set; }
        public string language { get; set; }
    }
}