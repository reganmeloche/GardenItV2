using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using gardenit_api_classes.Plant;
using gardenit_api_classes.Water;

namespace gardenit_webapi.Lib
{
    public interface IPlantLib
    {
        PlantResponse GetPlant(Guid id, Guid userId);
        List<PlantResponse> GetAllPlants(Guid userId);
        NewPlantResponse CreatePlant(NewPlantRequest request, Guid userId);
        void UpdatePlant(Guid id, UpdatePlantRequest request, Guid userId);
        void DeletePlant(Guid id, Guid userId);
    }
}