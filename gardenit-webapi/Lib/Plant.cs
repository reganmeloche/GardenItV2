using System;
using System.Collections.Generic;

using gardenit_api_classes.Plant;

namespace gardenit_webapi.Lib
{
    public class Plant : PlantBase
    {
        public Guid Id { get; set; }
        public List<Watering> Waterings { get; set; }
    }

    public class Watering {
        public DateTime WateringDate { get; set; }
    }
}