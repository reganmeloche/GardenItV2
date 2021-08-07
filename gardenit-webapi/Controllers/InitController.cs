using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using gardenit_webapi.Mqtt;
using gardenit_webapi.Storage;


namespace gardenit_webapi.Controllers
{
    // TODO: Need to trigger this to happen automatically, and then remove this...
    [ApiController]
    [Route("[controller]")]
    public class InitController : ControllerBase
    {
        private readonly IStorePlants _store;
        private readonly IMqttLib _mqttLib;
        private readonly IServiceScopeFactory _sf;

        public InitController(IStorePlants store, IMqttLib mqttLib, IServiceScopeFactory sf)
        {
            _store = store;
            _mqttLib = mqttLib;
            _sf = sf;
        }

        [HttpGet]
        public async Task<ActionResult> Init()
        {
            var plants = _store.GetAllPlants();
            var mqttHandler = HandlerCreator.Create(_sf);
            _mqttLib.SetHandler(mqttHandler);

            var plantIds = plants.Select(x => x.Id).ToList();
            await _mqttLib.Init(plantIds);
   
            return Ok();
        }

        private Guid UserId() {
            return Guid.Parse(HttpContext.Request.Headers["UserId"].FirstOrDefault());
        }
    }
}
