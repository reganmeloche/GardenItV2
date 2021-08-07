using System;
using System.Threading.Tasks;

using gardenit_api_classes.Moisture;
using gardenit_webapi.Storage;
using gardenit_webapi.Mqtt;

namespace gardenit_webapi.Lib
{
    public class MoistureLib : IMoistureLib
    {
        private readonly IStorePlants _storage;
        private readonly IMqttLib _mqttLib;

        public MoistureLib(IStorePlants storage, IMqttLib mqttLib) {
            _storage = storage;
            _mqttLib = mqttLib;
        }
        
        public async Task RequestReading(MoistureReadingRequest req) {
            // TODO: Get the plant and verif userId??
            string message = "RM";
            await _mqttLib.PublishMessage(req.PlantId, message);
        } 

        public void AddReading(Guid plantId, MoistureReading reading) {
            _storage.AddMoistureReading(plantId, reading);
        }

        public static MoistureReadingResponse Convert(MoistureReading reading) {
            return new MoistureReadingResponse() {
                ReadDate = reading.ReadDate,
                Value = reading.Value
            };
        }
    }
}