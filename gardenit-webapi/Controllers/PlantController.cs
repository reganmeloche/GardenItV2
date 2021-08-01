using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using gardenit_api_classes.Plant;
using gardenit_webapi.Lib;

namespace gardenit_webapi.Controllers
{
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
            var result = _lib.CreatePlant(request);
            return result;
        }

        [HttpGet("{id}")]
        public ActionResult<PlantResponse> GetPlant(Guid id) 
        {
            var result = _lib.GetPlant(id);
            return result;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlantResponse>> GetAllPlants() 
        {
            var result = _lib.GetAllPlants();
            return result;
        }

        [HttpPut("{id}")]
        public ActionResult UpdatePlant(Guid id, UpdatePlantRequest request) 
        {
            _lib.UpdatePlant(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePlant(Guid id) 
        {
            _lib.DeletePlant(id);
            return Ok();
        }
    }
}
