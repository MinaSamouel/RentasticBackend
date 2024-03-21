using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Repos;

namespace RentasticBackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarsController : ControllerBase
{
    private readonly ICarRepo _repo;
    private readonly IValidator<CarModel> _validator;

    readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true
    };

    public CarsController(ICarRepo repo, IValidator<CarModel> validator)
    {
        _repo = repo;
        _validator = validator;
    }

    [HttpGet] 
    public IActionResult GetAllCars()
    {
        var cars = _repo.GetAllCars();

        return Ok(cars);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("Admin")]
    public IActionResult GetAllCarsAdmin()
    {
        var cars = _repo.GetCarsAdmin();

        return Ok(JsonSerializer.Serialize(cars, _options));
    }

    [HttpGet("{id}")]
    public IActionResult GetCarById(int id)
    {
        if (!_repo.ExistsId(id))
            return NotFound("Ther is no Car with that Id");

        var car = _repo.GetById(id);
        return Ok(car);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("Admin/{id}")]
    public IActionResult GetCarByIdAdmin(int id)
    {
        if (!_repo.ExistsId(id))
            return NotFound("Ther is no Car with that Id");

        var car = _repo.GetCarAdmin(id);
        return Ok(JsonSerializer.Serialize(car, _options));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult AddCar([FromBody] CarModel carModel)
    {
        var result = _validator.Validate(carModel);

        if (!result.IsValid)
        {
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add("Validation Felis: " + error.ErrorMessage);
            }
            return BadRequest(errors);
        }

        var car = new Car
        {
            Name = carModel.Name,
            Brand = carModel.Brand,
            ModelYear = carModel.ModelYear,
            Color = carModel.Color,
            Category = carModel.Category,
            SeatCount = carModel.SeatCount,
            PricePerDay = carModel.PricePerDay,
            Images = carModel.Images,
            IsAutomatic = carModel.IsAutomatic,
            HasAirCondition = carModel.HasAirCondition
        };

        _repo.Add(car);

        return Ok("Car Added Successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult UpdateCar(int id, [FromBody] CarModel model)
    {
        if(!_repo.ExistsId(id))
            return NotFound("There is no Car with that Id");

        var result = _validator.Validate(model);
        if (!result.IsValid)
        {
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add("Validation Felis: " + error.ErrorMessage);
            }
            return BadRequest(errors);
        }

        var carUpdate = _repo.GetById(id);
        carUpdate!.Name = model.Name;
        carUpdate.Brand = model.Brand;
        carUpdate.ModelYear = model.ModelYear;
        carUpdate.Color = model.Color;
        carUpdate.Category = model.Category;
        carUpdate.SeatCount = model.SeatCount;
        carUpdate.PricePerDay = model.PricePerDay;
        carUpdate.Images = model.Images;
        carUpdate.IsAutomatic = model.IsAutomatic;
        carUpdate.HasAirCondition = model.HasAirCondition;

        _repo.Update(carUpdate);

        return Ok("Car Updated Successfully");
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCar(int id)
    {
        if (!_repo.ExistsId(id))
            return NotFound("There is no Car with that Id");

        var car = _repo.GetById(id);
        _repo.Delete(car!);

        return Ok("Car Deleted Successfully");
    }
}

