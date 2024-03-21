using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Repos;

namespace RentasticBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        //CarRentalContext carRentalContext=new CarRentalContext();
        private readonly IReviewRepo _reviewRepo;
        private readonly IValidator<ReviewModel> validator;

        public ReviewController(IReviewRepo reviewRepo,IValidator<ReviewModel> validator)
        {
            _reviewRepo = reviewRepo;
            this.validator = validator;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("Reviews")]
        public IActionResult GetAllReviews()
        {
            var reviews = _reviewRepo.GetAllReviews();
            //var reviews= carRentalContext.Reviews.ToList();
            return Ok(reviews);
        }

        [HttpGet("GetReviewById")]
        public IActionResult GetReviewById(int carId, int userSsn, int reservationId)
        {
            var review = _reviewRepo.GetReviewById(carId, userSsn, reservationId);
            if (review == null)
            {
                return NotFound("Review not found");
            }
            return Ok(review);
        }
        [Authorize]
        [HttpPost("AddReview")]
        public IActionResult AddReview([FromBody]ReviewModel model)
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
            var reviewToAdd = new Review
            {
                UserSsn = model.UserSsn,
                ReservationId = model.ReservationId,
                Rate = model.Rate,
                CarId = model.CarId,
                Message = model.Message
            };
            _reviewRepo.Add(reviewToAdd);
            return Ok("Review Added");
        }
        [Authorize]
        [HttpPut("EditReview")]
        public IActionResult EditReview(ReviewModel model)
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

            var existingReview = _reviewRepo.GetReviewById(model.CarId, model.UserSsn, model.ReservationId);

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
}
