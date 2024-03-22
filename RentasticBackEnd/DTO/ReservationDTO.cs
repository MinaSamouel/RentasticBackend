using FluentValidation;
using System;

namespace RentasticBackEnd.DTO
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public int UserSsn { get; set; }
        public int CarId { get; set; }
        public DateTime StartRentTime { get; set; }
        public DateTime EndRentDate { get; set; }
        public double TotalPrice { get; set; }
    }

    public class CarReservationValidator : AbstractValidator<ReservationDTO>
    {
        public CarReservationValidator()
        {
            RuleFor(x => x.UserSsn).NotEmpty().WithMessage("User SSN is required.");
            RuleFor(x => x.CarId).NotEmpty().WithMessage("Car ID is required.");
            RuleFor(x => x.StartRentTime).NotEmpty().WithMessage("Start rent time is required.")
                .GreaterThanOrEqualTo(DateTime.Now).WithMessage("Start rent time should not be in the past.");
            RuleFor(x => x.EndRentDate).NotEmpty().WithMessage("End rent date is required.")
                .GreaterThan(x => x.StartRentTime).WithMessage("End rent date should be after start rent time.");
            RuleFor(x => x.TotalPrice).GreaterThan(0).WithMessage("Total price should be greater than 0.");
        }
    }
}
