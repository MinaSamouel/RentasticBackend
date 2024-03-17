using FluentValidation;

namespace RentasticBackEnd.DTO;

public class LoginModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginValidator : AbstractValidator<LoginModel>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("This isn't a valid Email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Email is required")
            .MinimumLength(8).WithMessage("The Password should at least 8 character")
            .MaximumLength(25).WithMessage("Password Length shouldn't exceed 25 character");
    }
}