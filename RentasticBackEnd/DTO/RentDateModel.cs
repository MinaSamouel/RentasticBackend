using FluentValidation;

namespace RentasticBackEnd.DTO;

public class RentDateModel
{
    public string? StartYear { get; set; }
    public string? StartMonth { get; set; }
    public string? StartDay { get; set; }

    public string? EndYear { get; set; }
    public string? EndMonth { get; set; }
    public string? EndDay { get; set; }
}

public class RentDateValidator : AbstractValidator<RentDateModel>
{
    public RentDateValidator()
    {
        RuleFor(x => x.StartYear)
            .NotEmpty().WithMessage("Start Year is required")
            .Matches(@"^(20)\d{2}$").WithMessage("The Start Year must on form 2022 Or 2023 Or 2024");

        RuleFor(x => x.StartMonth)
            .NotEmpty().WithMessage("Start Month is required")
            .Matches(@"^(0?[1-9]|1[012])$").WithMessage("The Start Month must be between 01 and 12");

        RuleFor(x => x.StartDay)
            .NotEmpty().WithMessage("Start Day is required")
            .Matches(@"^(0?[1-9]|[12][0-9]|3[01])$").WithMessage("The Start Day must be between 01 and 31");

        RuleFor(x => x.EndYear)
            .NotEmpty().WithMessage("End Year is required")
            .Matches(@"^(20)\d{2}$").WithMessage("The Start Year must on form 2022 Or 2023 Or 2024");

        RuleFor(x => x.EndMonth)
            .NotEmpty().WithMessage("End Month is required")
            .Matches(@"^(0?[1-9]|1[012])$").WithMessage("The End Month must be between 01 and 12");

        RuleFor(x => x.EndDay)
            .NotEmpty().WithMessage("End Day is required")
            .Matches(@"^(0?[1-9]|[12][0-9]|3[01])$").WithMessage("The End Day must be between 01 and 31");
    }
}