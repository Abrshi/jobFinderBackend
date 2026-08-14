using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using jobFinderBackend.Application.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace jobFinderBackend.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(
        int userId,
        string email,
        string role)
    {
        // Get JWT settings
        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException(
                "JWT key is not configured."
            );
        }

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];


        // Claims
        var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()
            ),

            new Claim(
                JwtRegisteredClaimNames.Email,
                email
            ),

            new Claim(
                ClaimTypes.Role,
                role
            )
        };


        // Security key
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );


        // Signing credentials
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );


        // Create JWT
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );


        // Convert JWT to string
        var tokenHandler =
            new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }
}