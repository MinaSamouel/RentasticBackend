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
        private readonly ICarRepo _carRepo;

        readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };

        public UserController(IUserRepo userRepo, IValidator<RegisterModel> validator, UserManager<ApplicationUser> userManager, ICarRepo carRepo)
        {
            _userRepo = userRepo;
            _validator = validator;
            _userManager = userManager;
            _carRepo = carRepo;
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

        //[Authorize(Roles = "User")]
        [HttpGet("Logged/{guid}")]
        public async Task<IActionResult> GetUserId(string guid)
        {
            var loginUser = await _userManager.FindByIdAsync(guid);
            if(loginUser == null) 
                return NotFound("User isn't exist");

            var checkUser = _userRepo.GetOneByEmail(loginUser.Email!);
            var getUserfull = _userRepo.GetOneByIdUser(checkUser!.Ssn);

            //make the navigation property equal to null
            
            if (getUserfull!.Reservations.Count > 0)
            {
                foreach (var reservation in getUserfull.Reservations)
                {
                    reservation.User = null!;
                    reservation.Car = _carRepo.GetByIdForUserLogged(reservation.CarId);
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
            if (getUserfull!.FavoriteCars.Count > 0)
            {
                foreach (var favoriteCar in getUserfull.FavoriteCars)
                {
                    favoriteCar.User = null!;
                    favoriteCar.Car = null!;
                }
            }

            var returnedUser = new
            {
                Ssn = getUserfull!.Ssn,
                Name = getUserfull.Name,
                Email = getUserfull.Email,
                PhoneNumber = getUserfull.PhoneNumber,
                Address = getUserfull.Address,
                Image= getUserfull.Image,
                NationalIdentityNumber = getUserfull.NationalIdentityNumber,
                Reservations = getUserfull.Reservations,
                Reviews = getUserfull.Reviews,
                FavoriteCars = getUserfull.FavoriteCars,
                IsAdmin = getUserfull.IsAdmin,

            };
            return Ok(JsonSerializer.Serialize(returnedUser, _options));
        }

        [Authorize(Roles = "User")]
        [HttpPut("{guid}")]
        public async Task<IActionResult> UpdateUser(string guid, [FromBody] RegisterModel model)
        {
            //There is something missing here how to make sure user is updating his own data not someone else
            var userCheck = await _userManager.FindByIdAsync(guid);
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
            updatedUser.PhoneNumber = model.PhoneNumber;
            updatedUser.NationalIdentityNumber = model.NationalIdentityNumber;
            var finalUser = _userRepo.Update(updatedUser);

            return Ok(JsonSerializer.Serialize(finalUser, _options));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{guid}")]
        public async Task<IActionResult> DeleteUser(string guid)
        {
            var loginUser = await _userManager.FindByIdAsync(guid);
            if (loginUser == null)
                return NotFound("User Not exist");

            var checkUser = _userRepo.GetOneByEmail(loginUser.Email!);
            if (checkUser == null)
                return NotFound("There is no user with gived Number");

            _userRepo.Delete(checkUser);
            await _userManager.DeleteAsync(loginUser);

            return Ok("User Deleted");
        }

    }
}
