using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using gardenit_api_classes.Plant;
using gardenit_api_classes.Water;
using gardenit_webapi.Storage;
using gardenit_webapi.Mqtt;

namespace gardenit_webapi.Lib
{
    public class PlantLib : IPlantLib
    {
        private readonly IStorePlants _storage;
        private readonly IMqttLib _mqttLib;

        public PlantLib(IStorePlants storage, IMqttLib mqttLib) {
            _storage = storage;
            _mqttLib = mqttLib;
        }

        public NewPlantResponse CreatePlant(NewPlantRequest newPlant, Guid userId) {
            var plantToCreate = new Plant(newPlant);
            _storage.CreatePlant(plantToCreate, userId);

            return new NewPlantResponse() {
                Id = plantToCreate.Id
            };
        }

        public List<PlantResponse> GetAllPlants(Guid userId) {
            var result = new List<PlantResponse>();
            var storageResults = _storage.GetAllPlants(userId);

            foreach (Plant x in storageResults) {
                var nextPlant = Convert(x);
                result.Add(nextPlant);
            }
            return result;
        }

        public PlantResponse GetPlant(Guid id, Guid userId) {
            var plant = _storage.GetPlant(id, userId);
            return Convert(plant);
        }

        public void UpdatePlant(Guid id, UpdatePlantRequest request, Guid userId) {
            var plant = _storage.GetPlant(id, userId);
            bool pollPeriodChange = request.PollPeriodMinutes != plant.PollPeriodMinutes;

            plant.Name = request.Name;
            plant.Notes = request.Notes;
            plant.Type = request.Type;
            plant.DaysBetweenWatering = request.DaysBetweenWatering;
            plant.ImageName = request.ImageName;
            plant.PollPeriodMinutes = request.PollPeriodMinutes;
            plant.HasDevice = request.HasDevice;
            _storage.UpdatePlant(plant, userId);

            // Check polling period change
            if (pollPeriodChange) {
                string message = $"WP{request.PollPeriodMinutes}";
                _mqttLib.PublishMessage(id, message);
            }
        }

        public void DeletePlant(Guid id, Guid userId) {
            _storage.DeletePlant(id, userId);
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
                HasDevice = plant.HasDevice,
                PollPeriodMinutes = plant.PollPeriodMinutes,
                Waterings = plant.Waterings.Select(WateringLib.Convert).ToList(),
                MoistureReadings = plant.MoistureReadings.Select(MoistureLib.Convert).ToList()
            };
        }
    }
}