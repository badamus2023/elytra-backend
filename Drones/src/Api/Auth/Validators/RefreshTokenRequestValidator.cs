using Drones.src.Api.Auth.DTOs.Requests;
using FluentValidation;

namespace Drones.src.Api.Auth.Validators
{
    public class RefreshTokenRequestValidator: AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token is required");
        }
    }
}
