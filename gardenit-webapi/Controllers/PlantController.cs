using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

using gardenit_api_classes.Plant;
using gardenit_webapi.Lib;

namespace gardenit_webapi.Controllers
{
    [ServiceFilter(typeof(EncryptionFilterAttribute))]
    [ApiController]
    [Route("[controller]")]
    public class PlantController : ControllerBase
    {
        private readonly IPlantLib _lib;

        public PlantController(IPlantLib lib)
        {
            _lib = lib;
        }

        [HttpPost]
        public ActionResult<NewPlantResponse> CreatePlant(NewPlantRequest request)
        {
            var result = _lib.CreatePlant(request, UserId());
            return result;
        }

        [HttpGet("{id}")]
        public ActionResult<PlantResponse> GetPlant(Guid id) 
        {
            var result = _lib.GetPlant(id, UserId());
            return result;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlantResponse>> GetAllPlants() 
        {
            var userId = HttpContext.Request.Headers["UserId"].FirstOrDefault();
            var result = _lib.GetAllPlants(UserId());
            return result;
        }

        [HttpPut("{id}")]
        public ActionResult UpdatePlant(Guid id, UpdatePlantRequest request) 
        {
            _lib.UpdatePlant(id, request, UserId());
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePlant(Guid id) 
        {
            _lib.DeletePlant(id, UserId());
            return Ok();
        }

        private Guid UserId() {
            return Guid.Parse(HttpContext.Request.Headers["UserId"].FirstOrDefault());
        }
    }
}
