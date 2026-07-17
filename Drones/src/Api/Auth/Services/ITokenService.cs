using Drones.src.Api.Auth.Entities;

namespace Drones.src.Api.Auth.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, IList<string> roles);
    }
}
