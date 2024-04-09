using System.Security.Claims;

namespace KK.JWT
{
    public interface ITokenService
    {
        string BuildToken(IEnumerable<Claim> claims, JwtOptions options);
    }
}
