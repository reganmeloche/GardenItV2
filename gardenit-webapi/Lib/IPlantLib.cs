using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using gardenit_api_classes.Plant;
using gardenit_api_classes.Water;

namespace gardenit_webapi.Lib
{
    public interface IPlantLib
    {
        PlantResponse GetPlant(Guid id);
        List<PlantResponse> GetAllPlants();
        NewPlantResponse CreatePlant(NewPlantRequest request);
        void UpdatePlant(Guid id, UpdatePlantRequest request);
        void DeletePlant(Guid id);
    }
}