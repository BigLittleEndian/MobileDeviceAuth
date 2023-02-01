using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using DeviceAuth.Models;

namespace DeviceAuth.Security;

public static class JWTToken
{
   private static IConfiguration _configuration; // In production code check is configuration present and log error if not

   public static void Initialize(IConfiguration configuraton)
   {
      _configuration = configuraton;
   }

   public static Action<JwtBearerOptions> GetOptions(string sectionName)
   {
      var root = _configuration.GetSection(sectionName);

      return options =>
      {
         options.TokenValidationParameters = new TokenValidationParameters
         {
            ValidateActor = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = root.GetSection("Issuer").Value,
            ValidAudience = root.GetSection("Audience").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(root.GetSection("Key").Value!))
         };
      };
   }

   public enum LoginStatus
   {
      OK = 0,
      Unauthorized = 1,
      BadRequest = 2
   }

   public static (LoginStatus status, string? token) CreateToken(DeviceLogin deviceLogin, string configSection)
   {
      if (!string.IsNullOrEmpty(deviceLogin.SerialNumber) && !string.IsNullOrEmpty(deviceLogin.Secret))
      {
         if (deviceLogin.Secret != _configuration[$"{configSection}:Secret"])
         {
            return (LoginStatus.Unauthorized, null);
         }

         var claims = new[]
         {
            new Claim(ClaimTypes.SerialNumber, deviceLogin.SerialNumber),
            new Claim(ClaimTypes.Role, deviceLogin.SerialNumber == "11111" ? "Inspector" : "Employee") // Pull from DB role of device owner
        };

         var token = new JwtSecurityToken
         (
             issuer: _configuration[$"{configSection}:Issuer"],
             audience: _configuration[$"{configSection}:Audience"],
             claims: claims,
             expires: DateTime.UtcNow.AddMinutes(15),
             notBefore: DateTime.UtcNow,
             signingCredentials: new SigningCredentials(
                 new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[$"{configSection}:Key"]!)),
                                          SecurityAlgorithms.HmacSha256)
         );

         var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

         return (LoginStatus.OK, tokenString);
      }

      return (LoginStatus.BadRequest, null);
   }
}

