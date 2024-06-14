using AvtoMigBussines.Models;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        public CarController(ICarService carService)
        {
            _carService = carService;
        }
        [Route("DeleteCar")]
        [HttpPost]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var carForDelete = await _carService.GetCarByIdAsync(id);
            if (carForDelete != null)
            {
                carForDelete.IsDeleted = true;
                await _carService.UpdateCarAsync(carForDelete);
                return Ok(carForDelete);
            }
            return BadRequest();
        }
        [Route("UpdateCar")]
        [HttpPost]
        public async Task<IActionResult> UpdateCar([Required]int id, [FromBody] Car updateData)
        {
            var carToUpdate = await _carService.GetCarByIdAsync(id);
            if (carToUpdate != null)
            {
                carToUpdate.IsDeleted = false;
                carToUpdate.Name = updateData.Name;
                await _carService.UpdateCarAsync(carToUpdate);
                return Ok(carToUpdate);
            }
            return BadRequest();
        }

        [Route("ListCar")]
        [HttpGet]
        public async Task<IActionResult> ListCar()
        {
            var cars = await _carService.GetAllCarsAsync();
            if (cars == null)
            {
                return NotFound();
            }
            return Ok(cars);
        }
        [Route("CreateCar")]
        [HttpPost]
        public async Task<IActionResult> CreateCar(Car car)
        {
            if (ModelState.IsValid)
            {
                car.IsDeleted = false;
                await _carService.CreateCarAsync(car);
                return Ok(car);
            }
            return BadRequest();
        }
        [Route("GetCar")]
        [HttpGet]
        public async Task<IActionResult> GetCar([Required] int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car != null)
            {
                return Ok(car);
            }
            return BadRequest();
        }
    }
}
