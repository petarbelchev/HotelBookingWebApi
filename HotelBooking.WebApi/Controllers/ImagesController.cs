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

	// GET: api/hotels/5/images
	[HttpGet("~/api/hotels/{hotelId}/images")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<IEnumerable<ImageData>>> GetHotelImages(int hotelId)
		=> Ok(await imagesService.GetHotelImagesData(hotelId));

	// GET api/images/5
	[HttpGet("{id}")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<IEnumerable<ImageData>>> GetRoomImages(int roomId)
		=> Ok(await imagesService.GetRoomImagesData(roomId));

	// POST api/hotels/5/images
	[HttpPost("~/api/hotels/{hotelId}/images")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<IEnumerable<ImageData>>> CreateHotelImages(int hotelId, [Required][ValidImages] IFormFileCollection images)
	{
		try
		{
			IEnumerable<ImageData> outputModel = await imagesService.SaveHotelImages(hotelId, User.Id(), images);
			return CreatedAtAction(nameof(GetHotelImages), new { hotelId }, outputModel);
		}
		catch (KeyNotFoundException)
		{
			return BadRequest();
		}
	}

	// POST api/rooms/5/images
	[HttpPost("~/api/rooms/{roomId}/images")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult> CreateRoomImages(int roomId, [Required][ValidImages] IFormFileCollection images)
	{
		try
		{
			IEnumerable<ImageData> outputModel = await imagesService.SaveRoomImages(roomId, User.Id(), images);
			return CreatedAtAction(nameof(GetRoomImages), new { roomId }, outputModel);
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
	}

	// DELETE api/images/5
	[HttpDelete("{id}")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
