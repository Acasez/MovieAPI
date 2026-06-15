using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.DataTrransferObjects;
using MovieAPI.Models;
using MovieAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController(MovieInfoRepository repository, IMapper mapper) : ControllerBase
{
    // GET: api/Movies

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetCities(string? name, string? searchQuery)
    {
        IEnumerable<Movie>? movies = await repository.GetMoviesAsync();

        return Ok(mapper.Map<IEnumerable<MovieDTO>>(movies));
    }

    // GET: api/Movies/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MovieDTO>> GetMovie(int id)
    {
        Movie? city = await repository.GetMovieAsync(id);

        if (city == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<MovieDTO>(city));
    }

    // PUT: api/Movies/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMovie(int id, Movie movie)
    {
        if (id != movie.Id)
        {
            return BadRequest();
        }

        context.Entry(movie).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MovieExists(id))
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

    // POST: api/Movies
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Movie>> PostMovie(Movie movie)
    {
        context.Movies.Add(movie);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
    }

    // DELETE: api/Movies/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await context.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        context.Movies.Remove(movie);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool MovieExists(int id)
    {
        return context.Movies.Any(e => e.Id == id);
    }
}
