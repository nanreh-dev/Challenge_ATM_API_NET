using ATM.Models.CustomModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ATM.Helpers
{
    public class GetToken
    {
        //duracionn del token
        private static TimeSpan ExpiryDuration = new TimeSpan(3, 0, 0);
        public static string Get(string key, string issuer, LoginModel card)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.UserData, card.Card),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
            expires: DateTime.Now.Add(ExpiryDuration), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
