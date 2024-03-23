using FluentValidation;

namespace RentasticBackEnd.DTO;

public class FavoriteCarsModel
{
    public int CarId { get; set; }
    public string? UserGuid { get; set; }
}

public class FavoriteCarsValidator : AbstractValidator<FavoriteCarsModel>
{
    public FavoriteCarsValidator()
    {
        RuleFor(x => x.CarId)
            .NotEmpty().WithMessage("CarId is required")
            .GreaterThan(0).WithMessage("CarId must be greater than 0");

        RuleFor(x => x.UserGuid)
            .NotEmpty().WithMessage("UserGuid is required");
    }
}
