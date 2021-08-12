using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using gardenit_webapi.Mqtt;
using gardenit_webapi.Storage;

namespace gardenit_webapi.Lib
{
    public class AsyncInitializer
    {
        private readonly IStorePlants _store;
        private readonly IMqttLib _mqttLib;
        private readonly IServiceScopeFactory _sf;

        public AsyncInitializer(IStorePlants store, IMqttLib mqttLib, IServiceScopeFactory sf)
        {
            _store = store;
            _mqttLib = mqttLib;
            _sf = sf;
        }

        public async Task Initialize()
        {
            var plants = _store.GetAllPlants();
            var mqttHandler = HandlerCreator.Create(_sf);
            _mqttLib.SetHandler(mqttHandler);

            var plantIds = plants
                .Where(x => x.HasDevice)
                .Select(x => x.Id).ToList();

            await _mqttLib.Init(plantIds);
        }
    }
}