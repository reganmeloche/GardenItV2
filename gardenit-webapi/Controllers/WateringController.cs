using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using gardenit_api_classes.Plant;
using gardenit_api_classes.Water;
using gardenit_webapi.Lib;

namespace gardenit_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WateringController : ControllerBase
    {
        private readonly IHandlePlantRequests _handler;

        public WateringController(IHandlePlantRequests handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public ActionResult WaterPlant(WateringRequest request)
        {
            _handler.WaterPlant(request);
            return Ok();
        }
    }
}
