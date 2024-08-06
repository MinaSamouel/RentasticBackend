using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Models;
using RentasticBackEnd.Repos;

namespace RentasticBackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    //CarRentalContext carRentalContext=new CarRentalContext();
    private readonly IReviewRepo _reviewRepo;
    private readonly IValidator<ReviewModel> validator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepo _userRepo;
    private readonly byte[] _encryptionKey = Encoding.UTF8.GetBytes("your-encryption-key-here"); // Must be 16, 24, or 32 bytes for AES

    readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true
    };

    public ReviewController(IReviewRepo reviewRepo,IValidator<ReviewModel> validator, UserManager<ApplicationUser> userManager, IUserRepo userRepo)
    {
        _reviewRepo = reviewRepo;
        this.validator = validator;
        _userManager = userManager;
        _userRepo = userRepo;
    }

    //[Authorize(Roles = "Admin")]
    [HttpGet("Reviews")]
    public IActionResult GetAllReviews()
    {
        var reviews = _reviewRepo.GetAllReviews();
        //var reviews= carRentalContext.Reviews.ToList();
        return Ok(JsonSerializer.Serialize(reviews, _options));
    }

    private string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.GenerateIV();

        var iv = aes.IV;
        using var encryptor = aes.CreateEncryptor(aes.Key, iv);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        var encrypted = ms.ToArray();
        var result = new byte[iv.Length + encrypted.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

        return Convert.ToBase64String(result);
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("GetReviewById")]
    public async Task<IActionResult> GetReviewById(int carId, string userGuid, int reservationId)
    {
        var user = await  _userManager.FindByIdAsync(userGuid);
        if(user == null)
            return NotFound("User not found");
        var loggedUser = _userRepo.GetOneByEmail(user.Email!);
        var review = _reviewRepo.GetReviewById(carId, loggedUser!.Ssn, reservationId);

        if (review == null)
        {
            return NotFound("Review not found");
        }

        return Ok(JsonSerializer.Serialize(review, _options));
    }

    [Authorize(Roles = "User")]
    [HttpPost("AddReview")]
    public async Task<IActionResult> AddReview([FromBody]ReviewModel model)
    {  
        var result = validator.Validate(model);
        if(!result.IsValid) 
        {
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.ErrorMessage);
            }
            return BadRequest(errors);
        }

        var user = await _userManager.FindByIdAsync(model.UserGuid);
        if (user == null)
            return NotFound("User not found");
        var loggedUser = _userRepo.GetOneByEmail(user.Email!);

        var reviewToAdd = new Review
        {
            UserSsn = loggedUser!.Ssn,
            ReservationId = model.ReservationId,
            Rate = model.Rate,
            CarId = model.CarId,
            Message = model.Message
        };
        _reviewRepo.Add(reviewToAdd);
        return Ok("Review Added");
    }

    [Authorize(Roles = "User")]
    [HttpPut("EditReview")]
    public async Task<IActionResult> EditReview(ReviewModel model)
    {
        var result = validator.Validate(model);
        if (!result.IsValid)
        {
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.ErrorMessage);
            }
            return BadRequest(errors);
        }

        var user = await _userManager.FindByIdAsync(model.UserGuid);
        if (user == null)
            return NotFound("User not found");
        var loggedUser = _userRepo.GetOneByEmail(user.Email!);
        var existingReview = _reviewRepo.GetReviewById(model.CarId, loggedUser!.Ssn, model.ReservationId);
        if (existingReview == null)
        {
            return NotFound("Review not found");
        }

        existingReview.Rate = model.Rate;
        existingReview.Message = model.Message;
        _reviewRepo.Update(existingReview);

        return Ok("Review updated successfully");
    }

}

