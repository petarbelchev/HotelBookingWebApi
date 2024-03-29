﻿using HotelBooking.Services.RatingsService;
using HotelBooking.Services.RatingsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/ratings")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingsService ratingsService;

    public RatingsController(IRatingsService ratingsService)
        => this.ratingsService = ratingsService;

    // PUT api/comments/5/ratings
    [HttpPut("~/api/comments/{commentId}/ratings")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AvRatingOutputModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RateComment(int commentId, CreateRatingInputModel inputModel)
    {
        try
        {
            return Ok(await ratingsService.RateComment(commentId, User.Id(), inputModel));
        }
        catch (ArgumentException e)
        {
            ModelState.AddModelError(e.ParamName!, e.Message);
            return ValidationProblem();
        }
    }

    // PUT api/hotels/5/ratings
    [HttpPut("~/api/hotels/{hotelId}/ratings")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AvRatingOutputModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RateHotel(int hotelId, CreateRatingInputModel inputModel)
    {
        try
        {
            return Ok(await ratingsService.RateHotel(hotelId, User.Id(), inputModel));
        }
        catch (ArgumentException e)
        {
            ModelState.AddModelError(e.ParamName!, e.Message);
            return ValidationProblem();
        }
    }

    // PUT api/replies/5/ratings
    [HttpPut("~/api/replies/{replyId}/ratings")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AvRatingOutputModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RateReply(int replyId, CreateRatingInputModel inputModel)
    {
        try
        {
            return Ok(await ratingsService.RateReply(replyId, User.Id(), inputModel));
        }
        catch (ArgumentException e)
        {
            ModelState.AddModelError(e.ParamName!, e.Message);
            return ValidationProblem();
        }
    }
}
