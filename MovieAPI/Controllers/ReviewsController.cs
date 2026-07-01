using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;
using MovieAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieAPI.Interfaces;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController(IMovieService repository, IMapper mapper) : ControllerBase
{
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviews()
    {
        IEnumerable<Review> reviews = await repository.GetReviewsAsync();

        return Ok(mapper.Map<IEnumerable<ReviewDTO>>(reviews));
    }

    [HttpGet("{reviewId:int}")]
    public async Task<ActionResult<ReviewDTO>> GetReview(int reviewId)
    {
        Review? reviewEntity = await repository.GetReviewAsync(reviewId);

        if (reviewEntity == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<ReviewDTO>(reviewEntity));
    }

    [HttpPut("{reviewId:int}")]
    public async Task<IActionResult> UpdateReview(int reviewId, ReviewUpdateDTO review)
    {
        Review? reviewEntity = await repository.GetReviewAsync(reviewId);

        if (reviewEntity == null)
        {
            return NotFound();
        }

        mapper.Map(review, reviewEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDTO>> CreateReview(ReviewCreateDTO reviewToCreate)
    {
        Review? review = mapper.Map<Review>(reviewToCreate);

        await repository.CreateReview(review);
        await repository.SaveChangesAsync();

        ReviewDTO createdReview = mapper.Map<ReviewDTO>(review);
        return CreatedAtAction("GetReview", new { reviewId = createdReview.Id }, createdReview);
    }

    [HttpDelete("{reviewId:int}")]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        Review? review = await repository.GetReviewAsync(reviewId);
        if (review == null)
        {
            return NotFound();
        }

        repository.DeleteReview(review);
        await repository.SaveChangesAsync();

        return NoContent();
    }
}
