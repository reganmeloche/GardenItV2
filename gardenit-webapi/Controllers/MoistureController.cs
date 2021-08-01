using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using gardenit_api_classes.Moisture;
using gardenit_webapi.Lib;

namespace gardenit_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoistureController : ControllerBase
    {
        private readonly IMoistureLib _lib;

        public MoistureController(IMoistureLib lib)
        {
            _lib = lib;
        }

        [HttpPost("Request")]
        public async Task<ActionResult> RequestReading(MoistureReadingRequest request)
        {
            await _lib.RequestReading(request);
            return Ok();
        }
    }
}
