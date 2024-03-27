using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Models;
using RentasticBackEnd.Repos;

namespace RentasticBackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarsController : ControllerBase
{
    private readonly ICarRepo _repo;
    private readonly IValidator<CarModel> _validator;
    private readonly IValidator<RentDateModel> _dateValidator;

    readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true
    };

    public CarsController(ICarRepo repo, IValidator<CarModel> validator, IValidator<RentDateModel> dateValidator)
    {
        _repo = repo;
        _validator = validator;
        _dateValidator = dateValidator;
    }

    [HttpPost("ReservedTime")]
    public IActionResult GetCarsReserved([FromBody] RentDateModel rentTime)
    {
        var result = _dateValidator.Validate(rentTime);
        if (!result.IsValid)
        {
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add("Validation Felis: " + error.ErrorMessage);
            }
            return BadRequest(errors);
        }

        var startDate = new DateTime(int.Parse(rentTime.StartYear!), int.Parse(rentTime.StartMonth!),
            int.Parse(rentTime.StartDay!));

        var cars = _repo.GerCarRserved(startDate);

        return Ok(JsonSerializer.Serialize(cars, _options));
    }

    [Authorize(Roles = "User")]
    [HttpPost("AvailabeDate")]
    public IActionResult GetCarsAvailable([FromBody] RentDateModel rentTime)
    {
        var result = _dateValidator.Validate(rentTime);
        if (!result.IsValid)
        {
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add("Validation Felis: " + error.ErrorMessage);
            }
            return BadRequest(errors);
        }

        var startDate = new DateTime(int.Parse(rentTime.StartYear!), int.Parse(rentTime.StartMonth!),
                       int.Parse(rentTime.StartDay!));

        var cars = _repo.GerCarRserved(startDate);

        var allCars = _repo.GetAllCars();

        var availableCars = allCars.Except(cars).ToList();

        return Ok(JsonSerializer.Serialize(availableCars, _options));
    }

    [HttpGet] 
    public IActionResult GetAllCars()
    {
        var cars = _repo.GetAllCars();

        return Ok(JsonSerializer.Serialize(cars, _options));
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
        return Ok(JsonSerializer.Serialize(car, _options));
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
            HasAirCondition = carModel.HasAirCondition,
            Description = carModel.Description
        };
        _repo.Add(car);

        return Ok(new { message = "Car Added Successfully" });

    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult UpdateCar(int id, [FromBody] CarModel model)
    {
        if (!_repo.ExistsId(id))
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
        carUpdate.Description = model.Description;
        _repo.Update(carUpdate);

        return Ok(new { message = "Car Updated Successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteCar(int id)
    {
        if (!_repo.ExistsId(id))
            return NotFound("There is no Car with that Id");

        var car = _repo.GetById(id);
        _repo.Delete(car!);

        return Ok(new { message = "Car deleted Successfully" });
    }
}

