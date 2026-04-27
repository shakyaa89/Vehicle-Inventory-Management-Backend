using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using VehicleIMS_backend.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using VehicleIMS_backend.Application.Interfaces.IServices;
using VehicleIMS_backend.Infrastructure.Configurations;

namespace VehicleIMS_backend.Infrastructure.Services
{
    public class JwtTokenService(
    IOptions<JwtOptions> jwtOptions,
    UserManager<User> userManager) : IJwtTokenService
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        public async Task<string> GenerateUserToken(User user)
        {
            //creating claims list
            //will be used to create the payload later
            var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email!)
        };
            //adding roles dynamically to the claims list
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }
            //creates the actual token
            return GenerateToken(claims);
        }
        public string GenerateToken(IEnumerable<Claim> claims)
        {
            // will be used to create signature
            var credentials = new SigningCredentials(_jwtOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: _jwtOptions.ExpiryDate,
                signingCredentials: credentials);

            //creates the acutal jwt otken
            // header.payload.signature
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
