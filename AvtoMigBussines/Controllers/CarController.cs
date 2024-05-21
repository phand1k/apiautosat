using AvtoMigBussines.Models;
using AvtoMigBussines.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace AvtoMigBussines.Controllers
{
    public class CarController : Controller
    {
        private readonly CarService _carService;
        public CarController(CarService carService)
        {
            _carService = carService;
        }

        public IActionResult Index()
        {
            return View();
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
            if (ModelState.IsValid && await _carService.CreateCarAsync(car) != false)
            {
                await _carService.CreateCarAsync(car);
                return Ok(car);
            }
            return BadRequest();
        }
    }
}
