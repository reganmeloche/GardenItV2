using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using gardenit_api_classes.Plant;
using gardenit_api_classes.Water;
using gardenit_webapi.Storage;
using gardenit_webapi.Mqtt;

namespace gardenit_webapi.Lib
{
    public class WateringLib : IWateringLib
    {
        private readonly IStorePlants _storage;
        private readonly IMqttLib _mqttLib;

        public WateringLib(IStorePlants storage, IMqttLib mqttLib) {
            _storage = storage;
            _mqttLib = mqttLib;
        }

        public async Task WaterPlant(WateringRequest req, Guid userId) {
            int ms = req.Seconds * 1000;
            string message = $"WW{ms}";
            await _mqttLib.PublishMessage(req.PlantId, message);

            var watering = new Watering() {
                WateringDate = DateTime.Now,
                Seconds = req.Seconds
            };
            
            _storage.AddWatering(req.PlantId, watering, userId);
        } 

        public static WateringResponse Convert(Watering watering) {
            return new WateringResponse() {
                WateringDate = watering.WateringDate,
                Seconds = watering.Seconds
            };
        }
    }
}