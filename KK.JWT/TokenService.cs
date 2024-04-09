

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KK.JWT
{
    public class TokenService : ITokenService
    {
        public string BuildToken(IEnumerable<Claim> claims, JwtOptions options)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
            var token = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                expires: DateTime.Now.AddSeconds(options.ExpireSeconds),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
              );
            return jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
