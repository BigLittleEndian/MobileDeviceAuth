using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using DeviceAuth.Models;

namespace DeviceAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "JWTBearer01,JWTBearer02")]
public class TemperatureController : ControllerBase
{
   [HttpGet]
   [Route("")]
   public ActionResult<IEnumerable<Temperature>> GetTemperatures()
   {
      return Enumerable.Range(1, 5).Select(index => new Temperature
      {
         Time = TimeOnly.FromTimeSpan(TimeSpan.FromHours(index)),
         TemperatureC = Random.Shared.Next(15, 25)
      }).ToArray();
   }

   [HttpGet]
   [Route("Inspection")]
   [Authorize(Roles = "Inspector")]
   public ActionResult GetInspectionData()
   {
      return Ok("Data only for Inspectors");
   }
}
