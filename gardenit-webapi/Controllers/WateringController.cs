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
    [ServiceFilter(typeof(EncryptionFilterAttribute))]
    [ApiController]
    [Route("[controller]")]
    public class WateringController : ControllerBase
    {
        private readonly IWateringLib _lib;

        public WateringController(IWateringLib lib)
        {
            _lib = lib;
        }

        [HttpPost]
        public async Task<ActionResult> WaterPlant(WateringRequest request)
        {
            await _lib.WaterPlant(request, UserId());
            return Ok();
        }

        private Guid UserId() {
            return Guid.Parse(HttpContext.Request.Headers["UserId"].FirstOrDefault());
        }
    }
}
