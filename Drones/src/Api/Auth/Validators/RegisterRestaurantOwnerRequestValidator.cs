using Drones.src.Api.Auth.DTOs.Requests;
using FluentValidation;

namespace Drones.src.Api.Auth.Validators
{
    public class RegisterRestaurantOwnerRequestValidator : AbstractValidator<RegisterRestaurantOwnerRequest>
    {
        public RegisterRestaurantOwnerRequestValidator()
        {
            Include(new RegisterRequestValidator());
            RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.TaxId).NotEmpty().MaximumLength(32);
            RuleFor(x => x.ContactPhone).NotEmpty().MaximumLength(32);
            RuleFor(x => x.RestaurantName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(512);
            RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
            RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
            RuleFor(x => x.CloseTime).GreaterThan(x => x.OpenTime);
        }
    }
}
