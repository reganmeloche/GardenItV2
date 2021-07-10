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
        private readonly IHandlePlantRequests _handler;

        public PlantController(IHandlePlantRequests handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public ActionResult<NewPlantResponse> CreatePlant(NewPlantRequest request)
        {
            var result = _handler.CreatePlant(request);
            return result;
        }

        [HttpGet("{id}")]
        public ActionResult<PlantResponse> GetPlant(Guid id) 
        {
            var result = _handler.GetPlant(id);
            return result;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AllPlantsResponse>> GetAllPlants() 
        {
            var result = _handler.GetAllPlants();
            return result;
        }

        [HttpPut("{id}")]
        public ActionResult UpdatePlant(Guid id, UpdatePlantRequest request) 
        {
            _handler.UpdatePlant(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePlant(Guid id) 
        {
            _handler.DeletePlant(id);
            return Ok();
        }
    }
}
