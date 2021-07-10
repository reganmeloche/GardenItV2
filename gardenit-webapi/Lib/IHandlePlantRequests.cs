using System;
using System.Collections.Generic;

using gardenit_api_classes.Plant;
using gardenit_api_classes.Water;

namespace gardenit_webapi.Lib
{
    public interface IHandlePlantRequests
    {
        PlantResponse GetPlant(Guid id);
        List<AllPlantsResponse> GetAllPlants();
        NewPlantResponse CreatePlant(NewPlantRequest request);
        void UpdatePlant(Guid id, UpdatePlantRequest request);
        void DeletePlant(Guid id);
        void WaterPlant(WateringRequest req);
    }
}