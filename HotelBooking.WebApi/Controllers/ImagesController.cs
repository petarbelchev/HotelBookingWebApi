using HotelBooking.Services.ImagesService;
using HotelBooking.Services.ImagesService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/images")]
[ApiController]
public class ImagesController : ControllerBase
{
	private readonly IImagesService imagesService;

	public ImagesController(IImagesService imagesService)
		=> this.imagesService = imagesService;

	// GET: api/hotels/5/images
	[HttpGet("~/api/hotels/{hotelId}/images")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<ImageData>>> GetHotelImages(int hotelId)
		=> Ok(await imagesService.GetHotelImagesData(hotelId));

	// GET api/images/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ImageData>> GetImage(int id)
	{
		ImageData? image = await imagesService.GetImageData(id);

		return image != null 
			? Ok(image) 
			: NotFound();
	}

	// GET: api/rooms/5/images
	[HttpGet("~/api/rooms/{roomId}/images")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<ImageData>>> GetRoomImages(int roomId)
		=> Ok(await imagesService.GetRoomImagesData(roomId));

	// POST api/hotels/5/images
	[HttpPost("~/api/hotels/{hotelId}/images")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> CreateHotelImages(int hotelId, [FromForm] IFormFileCollection images)
	{
		try
		{
			await imagesService.SaveHotelImages(hotelId, User.Id(), images);
		}
		catch (KeyNotFoundException)
		{
			return BadRequest();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem(ModelState);
		}

		return NoContent();
	}

	// POST api/rooms/5/images
	[HttpPost("~/api/rooms/{roomId}/images")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> CreateRoomImages(int roomId, [FromForm] IFormFileCollection images)
	{
		try
		{
			await imagesService.SaveRoomImages(roomId, User.Id(), images);
		}
		catch (KeyNotFoundException)
		{
			return BadRequest();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem(ModelState);
		}

		return NoContent();
	}

	// DELETE api/images/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> Delete(int id)
	{
		try
		{
			await imagesService.DeleteImage(id, User.Id());
		}
		catch (KeyNotFoundException)
		{
			return BadRequest();
		}

		return NoContent();
	}
}
