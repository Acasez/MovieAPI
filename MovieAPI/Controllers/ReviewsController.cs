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
    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(int id)
    {
        var review = await context.Review.FindAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        return review;
    }

    // PUT: api/Reviews/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutReview(int id, Review review)
    {
        if (id != review.Id)
        {
            return BadRequest();
        }

        context.Entry(review).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReviewExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Reviews
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Review>> PostReview(Review review)
    {
        context.Review.Add(review);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetReview", new { id = review.Id }, review);
    }

    // DELETE: api/Reviews/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await context.Review.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        context.Review.Remove(review);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ReviewExists(int id)
    {
        return context.Review.Any(e => e.Id == id);
    }
}
