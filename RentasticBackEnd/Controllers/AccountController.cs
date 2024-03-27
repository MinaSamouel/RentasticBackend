using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Models;
using RentasticBackEnd.Repos;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace RentasticBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IValidator<RegisterModel> _registorValidator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<LoginModel> _loginValidator;
        private readonly IConfiguration _config;
        private readonly IUserRepo _userRepo;

        public AccountController(IValidator<RegisterModel> registorValidator, UserManager<ApplicationUser> userManager, IValidator<LoginModel> loginValidator, IConfiguration config, IUserRepo userRepo)
        {
            _registorValidator = registorValidator;
            _userManager = userManager;
            _loginValidator = loginValidator;
            _config = config;
            _userRepo = userRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var validationResult = _registorValidator.Validate(model);

            //Validate the data before sending it to the database
            if (!validationResult.IsValid)
            {
                var errors = new List<string>();
                foreach (var error in validationResult.Errors)
                {
                    errors.Add("Validation Field :" + error.ErrorMessage);
                }
                return BadRequest(errors);
            }

            //Check if the user already exists
            if (_userRepo.ExistsEmail(model.Email) || _userRepo.ExistsNationalNumber(model.NationalIdentityNumber))
            {
                return BadRequest("User Already Exists");
            }
            var filterName = model.Name!.Replace(" ", "");

            var userSign = new ApplicationUser
            {
                UserName = filterName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(userSign, model.Password!);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var userFromRegister = _userManager.FindByEmailAsync(model.Email).Result;
            var userToAdd = new User
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = userFromRegister!.PasswordHash,
                Address = model.Address,
                Image = model.Image,
                IsAdmin = false,
                NationalIdentityNumber = model.NationalIdentityNumber
            };
            _userRepo.Add(userToAdd);

            return Ok("User Created");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterModel model)
        {
            var validationResult = _registorValidator.Validate(model);

            //Validate the data before sending it to the database
            if (!validationResult.IsValid)
            {
                var errors = new List<string>();
                foreach (var error in validationResult.Errors)
                {
                    errors.Add("Validation Field :" + error.ErrorMessage);
                }

                return BadRequest(errors);
            }

            //Check if the user already exists
            if (_userRepo.ExistsEmail(model.Email) || _userRepo.ExistsNationalNumber(model.NationalIdentityNumber))
            {
                return BadRequest("User Already Exists");
            }

            //Create the user Identity and add it to the database
            var userSign = new ApplicationUser
            {
                UserName = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(userSign, model.Password!);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var userToAdd = new User
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                Address = model.Address,
                Image = model.Image!,
                IsAdmin = true,
                NationalIdentityNumber = model.NationalIdentityNumber
            };
            _userRepo.Add(userToAdd);

            return Ok("Admin Created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var validationResult = _loginValidator.Validate(model);

            //Validate the data before sending it to the database befor logining the user
            if (!validationResult.IsValid)
            {
                var errors = new List<string>();
                foreach (var error in validationResult.Errors)
                {
                    errors.Add("Validation Field :" + error.ErrorMessage);
                }
                return BadRequest(errors);
            }

            var loginUser = await _userManager.FindByEmailAsync(model.Email);

            if (loginUser == null || !await _userManager.CheckPasswordAsync(loginUser, model.Password))
            {
                return Unauthorized("Invalid Email Or Password");
            }

            var checkUser = _userRepo.GetOneByEmail(model.Email);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginUser.Id),
                new Claim(ClaimTypes.Name, checkUser!.Name!),
                new Claim(ClaimTypes.Email, loginUser.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Add the role for the user into the token
            if (checkUser != null && checkUser.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
                claims.Add(new Claim(ClaimTypes.Role, "User"));


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var signingCredentioal = new SigningCredentials(key: securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken myToken = new JwtSecurityToken(
                issuer: _config["Jwt:ValidIssuer"],
                audience: _config["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentioal
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(myToken),
                Expiration = myToken.ValidTo
            });
        }
        
    
    }
}
