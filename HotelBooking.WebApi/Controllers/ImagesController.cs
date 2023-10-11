using HotelBooking.Services.ImagesService;
using HotelBooking.Services.ImagesService.Models;
using HotelBooking.WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/images")]
[ApiController]
public class ImagesController : ControllerBase
{
	private readonly IImagesService imagesService;

	public ImagesController(IImagesService imagesService)
		=> this.imagesService = imagesService;

	// GET api/images/5
	[AllowAnonymous]
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetImage(int id)
	{
		ImageData? image = await imagesService.GetImageData(id);

		return image != null
			? File(image.Data, image.ContentType)
			: NotFound();
	}

	// POST api/hotels/5/images
	[HttpPost("~/api/hotels/{hotelId}/images")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SavedImagesOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> CreateHotelImages(
		int hotelId,
		[Required][ValidImages] IFormFileCollection images)
	{
		try
		{
			return Ok(await imagesService.SaveHotelImages(hotelId, User.Id(), images));
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
		}
	}

	// POST api/rooms/5/images
	[HttpPost("~/api/rooms/{roomId}/images")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SavedImagesOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> CreateRoomImages(
		int roomId,
		[Required][ValidImages] IFormFileCollection images)
	{
		try
		{
			return Ok(await imagesService.SaveRoomImages(roomId, User.Id(), images));
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
		}
	}

	// PUT api/hotels/5/images/5
	[HttpPut("~/api/hotels/{hotelId}/images/{imageId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> SetHotelMainImage(int imageId, int hotelId)
	{
		try
		{
			await imagesService.SetHotelMainImage(imageId, hotelId, User.Id());
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
		}

		return NoContent();
	}

	// PUT api/rooms/5/images/5
	[HttpPut("~/api/rooms/{roomId}/images/{imageId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> SetRoomMainImage(int imageId, int roomId)
	{
		try
		{
			await imagesService.SetRoomMainImage(imageId, roomId, User.Id());
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
		}

		return NoContent();
	}

	// DELETE api/images/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> Delete(int id)
	{
		try
		{
			await imagesService.DeleteImage(id, User.Id());
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}

		return NoContent();
	}
}
