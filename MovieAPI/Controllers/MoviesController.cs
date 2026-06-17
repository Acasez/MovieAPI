using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController(MovieInfoRepository repository, IMapper mapper) : ControllerBase
{
    // GET: api/Movies //TODO add string? name, string? searchQuery

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMovies()
    {
        IEnumerable<Movie> movies = await repository.GetMoviesAsync();

        return Ok(mapper.Map<IEnumerable<MovieDTO>>(movies));
    }

    // GET: api/Movies/5
    [HttpGet("{movieId}")]
    public async Task<ActionResult<MovieDTO>> GetMovie(int movieId)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);

        if (movieEntity == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<MovieDTO>(movieEntity));
    }

    // PUT: api/Movies/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{movieId}")]
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

        await repository.CreateMovie(movie, movieToCreate);
        await repository.SaveChangesAsync();

        MovieDTO createdMovie = mapper.Map<MovieDTO>(movie);
        return CreatedAtAction("GetMovie", new { movieId = createdMovie.Id }, createdMovie);
    }

    [HttpPatch("{movieId}")]
    public async Task<ActionResult> PartiallyUpdateMovie(int movieId, [FromBody]JsonPatchDocument<MovieUpdateDTO> patchDocument)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);
        if (movieEntity == null)
        {
            return NotFound();
        }

        MovieUpdateDTO moviePatch = mapper.Map<MovieUpdateDTO>(movieEntity);

        patchDocument.ApplyTo(moviePatch, jsonPatchError =>
        {
            string key = jsonPatchError.AffectedObject.GetType().Name;
            ModelState.AddModelError(key, jsonPatchError.ErrorMessage);
        });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(moviePatch))
        {
            return BadRequest(ModelState);
        }

        mapper.Map(moviePatch, movieEntity);
        await repository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Movies/5
    [HttpDelete("{movieId}")]
    public async Task<IActionResult> DeleteMovie(int movieId)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);

        if (movieEntity == null)
        {
            return NotFound();
        }

        repository.DeleteMovie(movieEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }
}
