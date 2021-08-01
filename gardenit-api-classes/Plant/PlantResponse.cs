using System;
using System.Collections.Generic;

using gardenit_api_classes.Water;
using gardenit_api_classes.Moisture;

namespace gardenit_api_classes.Plant
{
    public class PlantResponse : PlantResponseBase
    {
        public List<WateringResponse> Waterings { get; set; }
        public List<MoistureReadingResponse> MoistureReadings { get; set; }
    }
}