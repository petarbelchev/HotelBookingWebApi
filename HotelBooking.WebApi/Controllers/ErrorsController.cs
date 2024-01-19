using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

public class ErrorsController : ControllerBase
{
    [HttpGet("/error")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ErrorHandler() => Problem();
}
