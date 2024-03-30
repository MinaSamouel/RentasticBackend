using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Repos;

namespace RentasticBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IValidator<RegisterModel> _validator;
        private readonly UserManager<ApplicationUser> _userManager;

        readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };

        public UserController(IUserRepo userRepo, IValidator<RegisterModel> validator, UserManager<ApplicationUser> userManager)
        {
            _userRepo = userRepo;
            _validator = validator;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            
            var users = _userRepo.GetAllUsers();
            var serializedUsers = JsonSerializer.Serialize(users, _options);

            return Ok(serializedUsers);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            if (!_userRepo.ExistsId(id))
                return NotFound();

            var user = _userRepo.GetOneById(id);
            var serializedUser = JsonSerializer.Serialize(new
            {
                UserName = user!.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
            }, _options);

            return Ok(serializedUser);
        }

        [Authorize(Roles = "User")]
        [HttpGet("Logged/{id}")]
        public async Task<IActionResult> GetUserId(string id)
        {
            var loginUser = await _userManager.FindByIdAsync(id);
            if(loginUser == null) 
                return NotFound("User isn't exist");

            var checkUser = _userRepo.GetOneByEmail(loginUser.Email!);
            var getUserfull = _userRepo.GetOneByIdUser(checkUser!.Ssn);

            //make the navigation property equal to null
            if (getUserfull!.FavoriteCars.Count > 0)
            {
                foreach (var favoriteCar in getUserfull.FavoriteCars)
                {
                    favoriteCar.User = null!;
                }
            }
            if (getUserfull.Reservations.Count > 0)
            {
                foreach (var reservation in getUserfull.Reservations)
                {
                    reservation.User = null!;
                    reservation.Car = null!;
                    reservation.Review = null!;
                }
            }
            if (getUserfull.Reviews.Count > 0)
            {
                foreach (var review in getUserfull.Reviews)
                {
                    review.User = null!;
                    review.Car = null!;
                    review.Reservation= null!;
                }
            }

            var returnedUser = new
            {
                UserId = getUserfull!.Ssn,
                UserName = getUserfull.Name,
                Email = getUserfull.Email,
                PhoneNumber = getUserfull.PhoneNumber,
                Address = getUserfull.Address,
                Reservations = getUserfull.Reservations,
                Reviews = getUserfull.Reviews,
                FavouriteCars = getUserfull.FavoriteCars
            };
            return Ok(JsonSerializer.Serialize(returnedUser, _options));
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] RegisterModel model)
        {
            //There is something missing here how to make sure user is updating his own data no someone else
            var userCheck = await _userManager.FindByIdAsync(id);
            if (userCheck == null)
                return NotFound();

            var validationResult = _validator.Validate(model);
            if (!validationResult.IsValid)
            {
                var errors = new List<string>();
                foreach (var error in validationResult.Errors)
                {
                    errors.Add("Validation Field :" + error.ErrorMessage);
                }
                return BadRequest(errors);
            }

            userCheck.UserName = model.Name;
            await _userManager.UpdateAsync(userCheck);
            var updatedUser = _userRepo.GetOneByEmail(userCheck.Email!);
            updatedUser!.Name = model.Name;
            updatedUser.Address = model.Address;
            updatedUser.Image = model.Image;
            var finalUser = _userRepo.Update(updatedUser);

            return Ok(JsonSerializer.Serialize(finalUser, _options));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var checkUser = _userRepo.ExistsId(id);
            if (!checkUser)
                return NotFound();
            
            var user = _userRepo.GetOneById(id);
            var userSign = await _userManager.FindByEmailAsync(user!.Email);
            await _userManager.DeleteAsync(userSign!);
            _userRepo.Delete(user);

            return Ok("User Deleted");
        }

    }
}
