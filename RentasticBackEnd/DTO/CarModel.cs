using FluentValidation;

namespace RentasticBackEnd.DTO;

public class CarModel
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? ModelYear { get; set; }
    public string? Color { get; set; }
    public string? Category { get; set; }
    public int SeatCount { get; set; }
    public int PricePerDay { get; set; }
    public string? Images { get; set; }

    public bool IsAutomatic { get; set; }
    public bool HasAirCondition { get; set; } = true;
    public string? Description { get; set; }
}

public class CarValidator : AbstractValidator<CarModel>
{
    public CarValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(5,50).WithMessage("Name should be between 5 and 50 character");

        RuleFor(c => c.Brand)
            .NotEmpty().WithMessage("Brand is required")
            .Length(3, 50).WithMessage("Brand should be between 3 and 50 character");

        RuleFor(c => c.ModelYear)
            .NotEmpty().WithMessage("Model Year is required")
            .Matches(@"^(19|20)\d{2}$").WithMessage("The car model Year must on form 1999 Or 2015 Or 2006");

        RuleFor(c => c.Color)
            .NotEmpty().WithMessage("Color is required")
            .Length(3, 50).WithMessage("Color should be between 3 and 50 character");

        RuleFor(c => c.Category)
            .NotEmpty().WithMessage("Category is required")
            .Length(3, 50).WithMessage("Category should be between 3 and 50 character");

        RuleFor(c => c.SeatCount)
            .NotEmpty().WithMessage("Seat Count is required")
            .Must(v => v == 2 || v == 4 || v == 6).WithMessage("The seat Count must be 2 or 4 or 6");

        RuleFor(c => c.PricePerDay)
            .NotEmpty().WithMessage("Price Per Day is required")
            .GreaterThan(0).WithMessage("Price Per Day must be greater than 0");

        RuleFor(c => c.Images)
            .NotEmpty().WithMessage("Images is required");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Description is required")
            .Length(10, 500).WithMessage("Description should be between 10 and 500 character");
    }
}
