using FluentValidation;

namespace RentasticBackEnd.DTO;

public class RegisterModel
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Address { get; set; }
    public string? Image { get; set; } 
    public string NationalIdentityNumber { get; set; } = "";

}

public class RegisterValidator : AbstractValidator<RegisterModel>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("The Name should at least 3 character")
            .MaximumLength(25).WithMessage("Name Length shouldn't exceed 25 character");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("This isn't a valid Email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("The Password should at least 8 character")
            .MaximumLength(25).WithMessage("Password Length shouldn't exceed 25 character");

        RuleFor(x => x.Address)
            //.NotEmpty().WithMessage("Address is required")
            //.MinimumLength(10).WithMessage("The Address should at least 10 character")
            .MaximumLength(100).WithMessage("Address Length shouldn't exceed 100 character");

        RuleFor(user => user.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Length(11).WithMessage("Phone number must be exactly 11 characters.")
            .Matches(@"^(012|011|015|010)\d{8}$").WithMessage("Enter A valid Egyption Number");

        RuleFor(u => u.NationalIdentityNumber)
            .NotEmpty().WithMessage("National Identity Number is required.")
            .Length(14).WithMessage("National Identity Number must be 14 characters.")
            .Matches(@"^\d{14}$").WithMessage("The {PropertyName} must contain only digits and be 14 characters long.");

        //RuleFor(x => x.PhoneNumber)
        //    .NotEmpty().WithMessage("Phone Number is required")
        //    .MinimumLength(11).WithMessage("The Phone Number should at least 11 character")
        //    .MaximumLength(11).WithMessage("Phone Number Length shouldn't exceed 11 character")
        //    .Must(IsValidPhoneNumber)
        //    .WithMessage("Phone number must start with '012', '011', '015', or '010'."); 

        //RuleFor(x => x.Image)
        //    .NotEmpty().WithMessage("Image is required");

    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        // Check if the phone number starts with one of the specified prefixes
        return phoneNumber?.StartsWith("012") == true ||
               phoneNumber?.StartsWith("011") == true ||
               phoneNumber?.StartsWith("015") == true ||
               phoneNumber?.StartsWith("010") == true;
    }
}