using System;
using System.Collections.Generic;

using gardenit_webapi.Lib;

namespace gardenit_webapi.Storage
{
    public interface IStorePlants
    {
        void CreatePlant(Plant plant, Guid userId);

        List<Plant> GetAllPlants(Guid userId);
        List<Plant> GetAllPlants();

        Plant GetPlant(Guid id, Guid userId);

        void DeletePlant(Guid id, Guid userId);

        void UpdatePlant(Plant updatedPlant, Guid userId);

        void AddWatering(Guid plantId, Watering watering, Guid userId);

        void AddMoistureReading(Guid plantId, MoistureReading moistureReading);
    }
}