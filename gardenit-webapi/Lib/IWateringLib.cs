using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using gardenit_api_classes.Plant;
using gardenit_api_classes.Water;

namespace gardenit_webapi.Lib
{
    public interface IWateringLib
    {
        Task WaterPlant(WateringRequest req);
    }
}