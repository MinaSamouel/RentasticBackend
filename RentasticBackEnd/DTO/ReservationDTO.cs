using System;
using System.ComponentModel.DataAnnotations;

namespace RentasticBackEnd.DTO
{
    public class ReservationDTO
    {
        [Required(ErrorMessage = "User SSN is required.")]
        public string UserGuid { get; set; }

        [Required(ErrorMessage = "Car ID is required.")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Start rent time is required.")]
        [DataType(DataType.Date)]
        public DateTime StartRentTime { get; set; }

        [Required(ErrorMessage = "End rent date is required.")]
        [DataType(DataType.Date)]
        [EndDateMustBeGreaterThanStartDate(ErrorMessage = "End rent date should be after start rent time.")]
        public DateTime EndRentDate { get; set; }

        [Required(ErrorMessage = "Total price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price should be greater than 0.")]
        public double TotalPrice { get; set; }
    }

    public class EndDateMustBeGreaterThanStartDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (ReservationDTO)validationContext.ObjectInstance;

            if (model.StartRentTime >= model.EndRentDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
