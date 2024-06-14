using AvtoMigBussines.Models;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ModelCarController : Controller
    {
        private readonly IModelCarService _modelCarService;
        private readonly ICarService _carService;
        public ModelCarController(IModelCarService modelCarService, ICarService carService)
        {
            _modelCarService = modelCarService;
            _carService = carService;
        }
        [Route("ListModelCars")]
        [HttpGet]
        public async Task<IActionResult> ListModelCars([Required] int id)
        {
            var listModelCars = await _modelCarService.GetAllModelCarsAsync(id);
            if (listModelCars != null)
            {
                return Ok(listModelCars);
            }
            return BadRequest();
        }
        [Route("CreateModelCar")]
        [HttpPost]
        public async Task<IActionResult> CreateModelCar(int carId, ModelCar modelCar)
        {
            if (ModelState.IsValid)
            {
                var checkCarExists = await _carService.GetCarByIdAsync(carId);
                if (checkCarExists != null)
                {
                    modelCar.CarId = checkCarExists.Id;
                    modelCar.IsDeleted = false;
                    await _modelCarService.CreateModelCarAsync(modelCar);
                    return Ok(modelCar);
                }
                return NotFound("Car id is not valid");
            }
            return BadRequest();

        }

        [Route("GetModelCar")]
        [HttpGet]
        public async Task<IActionResult> GetModelCar(int id)
        {
            var modelCar = await _modelCarService.GetModelCarByIdAsync(id);
            if (modelCar != null)
            {
                return Ok(modelCar);
            }
            return BadRequest();
        }

        [Route("UpdateModelCar")]
        [HttpPost]
        public async Task<IActionResult> UpdateModelCar([Required] int id, [FromBody] ModelCar modelCar)
        {
            var modelCarExists = await _modelCarService.GetModelCarByIdAsync(id);
            var carExists = await _carService.GetCarByIdAsync((int)modelCar.CarId);

            if (modelCarExists != null && carExists != null)
            {
                modelCarExists.CarId = modelCar.CarId;
                modelCarExists.IsDeleted = false;
                modelCarExists.Name = modelCar.Name;
                await _modelCarService.UpdateModelCarAsync(modelCarExists);
                return Ok(modelCar);
            }
            return NotFound("Car or model car not found!");
        }
    }
}
