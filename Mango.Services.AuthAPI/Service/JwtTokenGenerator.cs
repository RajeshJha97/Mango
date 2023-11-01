using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.AuthAPI.Service
{

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _config;
        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var apiSettingConfig = _config.GetSection("ApiSettings:JwtOptions");
            var key = Encoding.ASCII.GetBytes(apiSettingConfig.GetValue<string>("SecretKey"));
            var issuer = apiSettingConfig.GetValue<string>("Issuer");
            var audience = apiSettingConfig.GetValue<string>("Audience");

            //step 1 create a list of claim u want in token
            var claimsList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email,applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub,applicationUser.Id), //in sub usually map with the user id
                new Claim(JwtRegisteredClaimNames.Name,applicationUser.UserName) //Name usually map with the username                
            };
            //Adding roles in the claimlist
            claimsList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            //step 2 token descriptor

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = audience,
                Issuer = issuer,
                Subject = new ClaimsIdentity(claimsList),//in subject we are passing claim list
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            //3: for token generator we need token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
