using System;
using System.Collections.Generic;

using gardenit_webapi.Lib;

namespace gardenit_webapi.Storage
{
    public interface IStorePlants
    {
        void CreatePlant(Plant plant);

        List<Plant> GetAllPlants();

        Plant GetPlant(Guid id);

        void DeletePlant(Guid id);

        void UpdatePlant(Plant updatedPlant);

        void AddWatering(Guid plantId, Watering watering);

        void AddMoistureReading(Guid plantId, MoistureReading moistureReading);
    }
}