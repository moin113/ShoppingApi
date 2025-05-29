using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyntraClone.API.Models;

namespace MyntraClone.API.Services
{
    public class JwtService
    {
        private readonly string _key;
        private readonly string _issuer;

        public JwtService(IConfiguration config)
        {
            // Get configuration values safely with null checks
            _key = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured");
            _issuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not configured");

            // Log key length for debugging (not the actual key for security)
            Console.WriteLine($"🔐 JWT Signing Key length: {_key.Length} chars");
        }

        public string GenerateToken(User user)
        {
            Console.WriteLine($"🔐 Generating JWT for User: {user.Email}, ID: {user.Id}, Role: {user.Role}");

            var claims = new[]
            {
                // Standard ClaimTypes for proper identity mapping
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Standard subject identifier
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),   // Used by GetUserId()
                new Claim(ClaimTypes.Email, user.Email),                   // Maps to User.FindFirst(ClaimTypes.Email)
                new Claim(ClaimTypes.Role, user.Role),                     // Maps to User.IsInRole and [Authorize(Roles=)]

                // Add ClaimTypes.Name for User.Identity.Name
                new Claim(ClaimTypes.Name, user.Id.ToString())             // Critical for User.Identity.Name
            };

            // Create the security key from the _key string
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            // Create signing credentials using HmacSha256 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token with all parameters
            var token = new JwtSecurityToken(
                issuer: _issuer,                   // Who issued the token
                audience: _issuer,                 // Who the token is intended for (using issuer as audience)
                claims: claims,                    // User claims/data
                expires: DateTime.UtcNow.AddDays(7), // Token valid for 7 days
                signingCredentials: creds          // Signature to verify token authenticity
            );

            // Log token expiration time for debugging
            Console.WriteLine($"🔐 Token Expiry Date: {token.ValidTo}");

            // Convert token object to JWT string format
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}