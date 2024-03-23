using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Repos;


namespace RentasticBackEnd.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FavouriteCarsController : ControllerBase
{
    private readonly IFavouriteCarRepo _repo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<FavoriteCarsModel> _validator;
    private readonly IUserRepo _userRepo;

    public FavouriteCarsController(IFavouriteCarRepo repo, UserManager<ApplicationUser> userManager, IValidator<FavoriteCarsModel> validator, IUserRepo userRepo)
    {
        _repo = repo;
        _userManager = userManager;
        _validator = validator;
        _userRepo = userRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FavoriteCarsModel model)
    {
        var result = _validator.Validate(model);
        if (!result.IsValid)
        {
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add("Validation Failed" + error.ErrorMessage);
            }
            return BadRequest(errors);
        }

        var userLogged = await _userManager.FindByIdAsync(model.UserGuid!);
        if (userLogged == null)
        {
            return Unauthorized();
        }

        var actualUser = _userRepo.GetOneByEmail(userLogged.Email!);
        _repo.HandleFavourite(model.CarId, actualUser!.Ssn);

        return Ok("Favourite Handled");
    }
}

