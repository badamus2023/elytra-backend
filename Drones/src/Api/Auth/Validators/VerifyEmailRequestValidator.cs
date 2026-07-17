using Drones.src.Api.Auth.DTOs.Requests;
using FluentValidation;

namespace Drones.src.Api.Auth.Validators
{
    public class VerifyEmailRequestValidator: AbstractValidator<VerifyEmailRequest>
    {
        public VerifyEmailRequestValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token is required");
        }
    }
}
