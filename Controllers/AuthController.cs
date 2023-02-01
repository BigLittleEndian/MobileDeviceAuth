using System;
using System.Text;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using DeviceAuth.Models;
using DeviceAuth.Security;

namespace DeviceAuth.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : Controller
{
   [HttpPost]
   [Route("GetToken01")]
   public IActionResult GetToken01([FromBody] DeviceLogin deviceLogin)
   {
      return CreateToken(deviceLogin, "JWTBearer01");
   }

   [HttpPost]
   [Route("GetToken02")]
   public IActionResult GetToken02([FromBody] DeviceLogin deviceLogin)
   {
      return CreateToken(deviceLogin, "JWTBearer02");
   }

   private IActionResult CreateToken(DeviceLogin deviceLogin, string configSection)
   {
      var result = JWTToken.CreateToken(deviceLogin, configSection);

      if (result.status == JWTToken.LoginStatus.Unauthorized)
      {
         return Unauthorized("Wrong secret provided.");
      }
      else if (result.status == JWTToken.LoginStatus.BadRequest)
      {
         return BadRequest("Input is not correct.");
      }

      return Ok(result.token);
   }

   [HttpGet]
   [Route("AboutMe")]
   public IActionResult AboutMe()
   {
      var aboutUser = "Not Authenticated";

      if (HttpContext.User.Identity is ClaimsIdentity identity)
      {
         if (identity.IsAuthenticated) // Works only for default scheme JWTBearer01
         {
            var claims = identity.Claims;

            if(claims.Any())
            {
               var serialNumber = claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
               var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

               aboutUser = $"SN: {serialNumber}, Role: {role}";
            }
         }
      }

      return Ok(aboutUser);
   }
}

