using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

public class ErrorsController : ControllerBase
{
	[Route("/error")]
	public IActionResult ErrorHandler() => Problem();
}
