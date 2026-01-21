using StudyMate.API.Contracts;
using StudyMate.API.Models.ModelsAuth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace StudyMate.API.Services
{
    public class JwtTokenGenerator :IJwtTokenGenerator
    {
        private readonly IConfiguration _config;

        // This class is incomplete, but the structure is what the compiler needs to find
        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            // 1. Prepare the Secret Key
            // Read the secret key from the configuration and convert it to bytes.
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!)
            );

            // 2. Define the Signing Credentials
            // This uses the key and the HMACSHA256 algorithm to create the token's signature.
            var signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            // 3. Define the Token Claims (The Payload Data)
            // These claims contain the identity information (UserID) that your server trusts.
            var claims = new List<Claim>
            {
             // Use ClaimTypes.NameIdentifier for the User's ID
             new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
             // Use ClaimTypes.Name for the UserName
             new Claim(ClaimTypes.Name, user.UserName),
        
            // Add other simple claims if needed, like roles
            // new Claim(ClaimTypes.Sender, "User") 
            };

            // 4. Create the Token Descriptor
            // This defines the token's metadata: expiration, issuer, audience, and claims.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Token expires in 7 days (standard practice)
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                SigningCredentials = signingCredentials
            };

            // 5. Generate and Serialize the Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // WriteToken serializes the token object into the final JWT string (header.payload.signature)
            return tokenHandler.WriteToken(token);
        }
    }
}
