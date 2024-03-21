using FluentValidation;

namespace RentasticBackEnd.DTO
{
    public class ReviewModel
    {
        public int ReservationId { get; set; }
        public int CarId { get; set; }
        public int UserSsn { get; set; }

        public string Message { get; set; } = null!;
        public int Rate { get; set; }
    }

    public class ReviewValidator : AbstractValidator<ReviewModel>
    {
        public ReviewValidator()
        {
            RuleFor(r => r.ReservationId)
                .NotEmpty().WithMessage("ReservationId is required")
                .GreaterThan(0).WithMessage("The ReservationId must be Greater Than zero");

            RuleFor(r => r.CarId)
                .NotEmpty().WithMessage("CarId is required")
                .GreaterThan(0).WithMessage("The CarId must be Greater Than zero");

            RuleFor(r => r.UserSsn)
                .NotEmpty().WithMessage("UserSsn is required")
                .GreaterThan(0).WithMessage("The UserSsn must be Greater Than zero");

            RuleFor(r => r.Message)
                .NotEmpty().WithMessage("Message is required")
                .MaximumLength(500).WithMessage("The Message must be less than 500 characters");

            RuleFor(r => r.Rate)
                .NotEmpty().WithMessage("Rate is required")
                .InclusiveBetween(1, 5).WithMessage("The Rate must be between 1 and 5");
        }
    }
}
