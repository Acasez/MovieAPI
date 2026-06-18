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

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController(MovieInfoRepository repository, IMapper mapper) : ControllerBase
{
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviews()
    {
        IEnumerable<Review> movies = await repository.GetReviewsAsync();

        return Ok(mapper.Map<IEnumerable<ReviewDTO>>(movies));
    }

    // GET: api/Reviews/5
    [HttpGet("{reviewID}")]
    public async Task<ActionResult<MovieDTO>> GetReview(int reviewID)
    {
        Review? actorEntity = await repository.GetReviewAsync(reviewID);

        if (actorEntity == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<MovieDTO>(actorEntity));
    }

    // PUT: api/Reviews/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{reviewId}")]
    public async Task<IActionResult> UpdateMovie(int reviewId, MovieUpdateDTO movie)
    {
        Movie? movieEntity = await repository.GetMovieAsync(reviewId);

        if (movieEntity == null)
        {
            return NotFound();
        }

        mapper.Map(movie, movieEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Reviews
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ReviewDTO>> CreateReview(ReviewCreateDTO reviewToCreate)
    {
        Review? review = mapper.Map<Review>(reviewToCreate);

        await repository.CreateReview(review);
        await repository.SaveChangesAsync();

        ReviewDTO createdReview = mapper.Map<ReviewDTO>(review);
        return CreatedAtAction("GetReview", new { reviewId = createdReview.Id }, createdReview);
    }

    // DELETE: api/Reviews/5
    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        var review = await repository.GetReviewAsync(reviewId);
        if (review == null)
        {
            return NotFound();
        }

        repository.DeleteReview(review);
        await repository.SaveChangesAsync();

        return NoContent();
    }
}
