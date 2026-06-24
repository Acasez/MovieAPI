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
    // GET: api/Movies 
    //TODO add string? name, string? searchQuery
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMovies(string? name, string? searchQuery)
    {
        IEnumerable<Movie> movies = await repository.GetMoviesAsync(name, searchQuery);

        return Ok(mapper.Map<IEnumerable<MovieDTO>>(movies));
    }

    // GET: api/Movies/movieId
    [HttpGet("{movieId:int}")]
    public async Task<ActionResult<MovieDTO>> GetMovie(int movieId)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);

        if (movieEntity == null)
        {
            return NotFound("Movie not found");
        }

        return Ok(mapper.Map<MovieDTO>(movieEntity));
    }

    // PUT: api/Movies/movieId
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{movieId}")]
    public async Task<IActionResult> UpdateMovie(int movieId, MovieUpdateDTO movie)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);

        if (movieEntity == null)
        {
            return NotFound("Movie not found");
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

    // POST: api/Movies/movieId
    [HttpPost("{movieId:int}/actor")]  // Create actor in movie
    public async Task<ActionResult<ActorDTO>> CreateActorInMovie(ActorCreateDTO actorToCreate, int movieId)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);
        if (movieEntity == null)
        {
            return NotFound();
        }

        Actor? actor = mapper.Map<Actor>(actorToCreate);

        await repository.CreateActor(actor, movieEntity);
        await repository.SaveChangesAsync();

        ActorDTO createdActorDto = mapper.Map<ActorDTO>(actor);
        return CreatedAtAction(actionName: "GetActor", controllerName: "Actors",
            routeValues: new { actorId = createdActorDto.Id },value: createdActorDto);
    }
    [HttpPost("{movieId:int}/details")]  // Create movieDetails
    public async Task<ActionResult<ActorDTO>> CreateMovieDetails(MovieDetailsCreateDTO movieDetailsToCreate, int movieId)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);
        if (movieEntity == null)
        {
            return NotFound();
        }

        MovieDetails? details = mapper.Map<MovieDetails>(movieDetailsToCreate);

        repository.CreateMovieDetails(details, movieEntity);
        await repository.SaveChangesAsync();

        MovieDetailsDTO createdDetailsDto = mapper.Map<MovieDetailsDTO>(details);
        return CreatedAtAction(actionName: "GetMovieDetails", controllerName: "Movies",
            routeValues: new { movieId = movieEntity.Id },value: createdDetailsDto);
    }

    [HttpPatch("{movieId:int}")]
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

        if (!ModelState.IsValid || !TryValidateModel(moviePatch))
        {
            return BadRequest(ModelState);
        }

        mapper.Map(moviePatch, movieEntity);
        await repository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Movies/movieId
    [HttpDelete("{movieId:int}")]
    public async Task<IActionResult> DeleteMovie(int movieId)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);
        if (movieEntity == null)
        {
            return NotFound("Movie not found");
        }

        repository.DeleteMovie(movieEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{movieId:int}/details")]
    public async Task<ActionResult<MovieDTO>> GetMovieDetails(int movieId)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);
        if (movieEntity == null)
        {
            return NotFound("Movie not found");
        }
        MovieDetails? movieDetails = await repository.GetMovieDetailByMovieIdAsync(movieId);
        if (movieDetails == null)
        {
            return NotFound("Movie details not found for movie id " + movieId);
        }

        return Ok(new {
            Movie = mapper.Map<MovieDTO>(movieEntity),
            Details = mapper.Map<MovieDetailsDTO>(movieDetails)
        });
    }
    
    [HttpDelete("details/{movieDetailsId:int}")]
    public async Task<IActionResult> DeleteMovieDetails(int movieDetailsId)
    {
        MovieDetails? movieDetails = await repository.GetMovieDetailAsync(movieDetailsId);

        if (movieDetails == null)
        {
            return NotFound();
        }

        repository.DeleteMovieDetails(movieDetails);
        await repository.SaveChangesAsync();

        return NoContent();
    }
}
