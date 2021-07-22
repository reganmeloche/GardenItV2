using System;
using System.Collections.Generic;
using System.Linq;

using gardenit_api_classes.Plant;
using gardenit_api_classes.Water;
using gardenit_webapi.Storage;

namespace gardenit_webapi.Lib
{
    // This may just turn into the lib
    public class PlantRequestHandler : IHandlePlantRequests
    {
        private readonly IStorePlants _storage;

        public PlantRequestHandler(IStorePlants storage) {
            _storage = storage;
        }

        public NewPlantResponse CreatePlant(NewPlantRequest newPlant) {
            var plantToCreate = new Plant() {
                Id = Guid.NewGuid(),
                Notes = newPlant.Notes,
                Name = newPlant.Name,
                Type = newPlant.Type,
                DaysBetweenWatering = newPlant.DaysBetweenWatering,
                ImageName = newPlant.ImageName,
                CreateDate = DateTime.Now,
                Waterings = new List<Watering>()
            };
            _storage.CreatePlant(plantToCreate);

            return new NewPlantResponse() {
                Id = plantToCreate.Id
            };
        }

        public List<PlantResponse> GetAllPlants() {
            var result = new List<PlantResponse>();
            var storageResults = _storage.GetAllPlants();

            foreach (Plant x in storageResults) {
                var nextPlant = Convert(x);
                result.Add(nextPlant);
            }
            return result;
        }

        public PlantResponse GetPlant(Guid id) {
            var plant = _storage.GetPlant(id);
            return Convert(plant);
        }

        public void UpdatePlant(Guid id, UpdatePlantRequest request) {
            var plant = _storage.GetPlant(id);
            plant.Name = request.Name;
            plant.Notes = request.Notes;
            plant.Type = request.Type;
            plant.DaysBetweenWatering = request.DaysBetweenWatering;
            plant.ImageName = request.ImageName;
            _storage.UpdatePlant(plant);
        }

        public void DeletePlant(Guid id) {
            _storage.DeletePlant(id);
        }

        public void WaterPlant(WateringRequest req) {
            _storage.AddWatering(req.PlantId);
        } 

        private static WateringResponse Convert(Watering watering) {
            return new WateringResponse() {
                WateringDate = watering.WateringDate
            };
        }

        private static PlantResponse Convert(Plant plant) {
            return new PlantResponse() {
                Id = plant.Id,
                Name = plant.Name,
                Notes = plant.Notes,
                Type = plant.Type,
                ImageName = plant.ImageName,
                DaysBetweenWatering = plant.DaysBetweenWatering,
                CreateDate = plant.CreateDate,
                Waterings = plant.Waterings.Select(x => Convert(x)).ToList(),
            };
        }
    }
}