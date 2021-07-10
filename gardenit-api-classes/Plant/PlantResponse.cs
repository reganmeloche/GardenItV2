using System;
using System.Collections.Generic;

using gardenit_api_classes.Water;

namespace gardenit_api_classes.Plant
{
    public class PlantResponse : PlantResponseBase
    {
        public List<WateringResponse> Waterings { get; set; }
    }
}