using System;
using System.Collections.Generic;

using gardenit_api_classes.Water;

namespace gardenit_api_classes.Plant
{
    public class PlantResponseBase : PlantBase
    {
        public Guid Id { get; set; }
    }
}