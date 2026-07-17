using Drones.src.Api.Auth.DTOs.Requests;
using FluentValidation;

namespace Drones.src.Api.Auth.Validators
{
    public class LoginRequestValidator: AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}
