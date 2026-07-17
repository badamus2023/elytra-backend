using Drones.src.Api.Auth.DTOs.Requests;
using FluentValidation;

namespace Drones.src.Api.Auth.Validators
{
    public class ResetPasswordRequestValidator: AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        { 
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token is required");

            RuleFor(x=> x.NewPassword).NotEmpty().WithMessage("New Password is required")
                .MinimumLength(8).WithMessage("New Password must be at least 8 characters long")
                .Matches("[A-Z]").WithMessage("New Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("New Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("New Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("New Password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password is required").Equal(x => x.NewPassword).WithMessage("Passwords do not match");
        }
    }
}
