using System;

namespace gardenit_api_classes.Water
{
    public class WateringRequest
    {
        public Guid PlantId { get; set; }
        public int Seconds { get; set; }
    }
}