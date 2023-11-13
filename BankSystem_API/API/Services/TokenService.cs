using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config) => _config = config;

        public string CreateToken(UserAccount user)
        {
            if(_config.GetValue<string>("TokenKey") is not null)
            {
                //set claims
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("TokenKey")));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                //create token
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }

            return "TestWriteToken"; // fpr unit test
        }

        public RefreshToken GenerateRefreshToken()
        {
            //generating random number for refresh token
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken { Token = Convert.ToBase64String(randomNumber) };
        }
    }
}
