using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController(MovieInfoRepository repository, IMapper mapper) : ControllerBase
{
    // GET: api/Movies

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetCities(string? name, string? searchQuery)
    {
        IEnumerable<Movie> movies = await repository.GetMoviesAsync();

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
    public async Task<IActionResult> UpdateMovie(int movieId, MovieUpdateDTO movie)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);

        if (movieEntity == null)
        {
            return NotFound();
        }

        mapper.Map(movie, movieEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Movies
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<MovieDTO>> CreateMovie(MovieCreateDTO movieToCreate)
    {
        Movie? movie = mapper.Map<Movie>(movieToCreate);

        await repository.CreateMovie(movie);
        await repository.SaveChangesAsync();

        MovieDTO createdMovie = mapper.Map<MovieDTO>(movie);

        return CreatedAtAction("GetMovie", new { id = createdMovie.Id }, createdMovie);
    }

    // DELETE: api/Movies/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int movieId)
    {
        Movie? movieEntitiy = await repository.GetMovieAsync(movieId);
        if (movieEntitiy == null)
        {
            return NotFound();
        }
        repository.DeleteMovie(movieEntitiy);
        await repository.SaveChangesAsync();

        return NoContent();
    }
}
