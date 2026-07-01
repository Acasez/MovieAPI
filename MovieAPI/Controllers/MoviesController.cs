using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DataTransferObjects;
using MovieAPI.Interfaces;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController(IMovieService repository, IMapper mapper, ILogger<MoviesController> logger) : ControllerBase
{
    ///<summary>
    /// Get all movies
    /// </summary> 
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMovies(string? name = "", string? searchQuery = "")
    {
        IEnumerable<Movie> movies = await repository.GetMoviesAsync(name, searchQuery);
        
        logger.LogInformation("Fetching movies");
        return Ok(mapper.Map<IEnumerable<MovieDTO>>(movies));
    }
    
    ///<summary>
    /// Get a movie by id
    /// </summary> 
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
    
    ///<summary>
    /// Get a movie by title
    /// </summary> 
    [HttpGet("{movieTitle}")]
    public async Task<ActionResult<MovieDTO>> GetMovie(string movieTitle)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieTitle);

        if (movieEntity == null)
        {
            return NotFound("Movie not found");
        }

        return Ok(mapper.Map<MovieDTO>(movieEntity));
    }
    
    ///<summary>
    /// Get all actors in a movie from movie id
    /// </summary> 
    [HttpGet("{movieId:int}/actors")]
    public async Task<ActionResult<IEnumerable<ActorDTO>>>GetMovieActors(int movieId)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);

        if (movieEntity == null)
        {
            return NotFound("Movie not found");
        }
        
        IEnumerable<Actor>? actors = movieEntity.Actors;
        if (actors == null || !actors.Any())
        {
            return NotFound("Movie has no actors listed");
        }

        return Ok(mapper.Map<IEnumerable<ActorDTO>>(actors));
    }
    
    ///<summary>
    /// Update a movie by id
    /// </summary> 
    [HttpPut("{movieId:int}")]
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
    
    ///<summary>
    /// Create a movie
    /// </summary> 
    [HttpPost]
    public async Task<ActionResult<MovieDTO>> CreateMovie(MovieCreateDTO movieToCreate)
    {
        if (await repository.GetMovieAsync(movieToCreate.Title) != null)
        {
            return BadRequest("Movie with Title already exists");
        }
                
        Movie? movie = mapper.Map<Movie>(movieToCreate);

        await repository.CreateMovie(movie, movieToCreate);
        await repository.SaveChangesAsync();

        MovieDTO createdMovie = mapper.Map<MovieDTO>(movie);
        return CreatedAtAction("GetMovie", new { movieId = createdMovie.Id }, createdMovie);
    }
    
    ///<summary>
    /// Create an actor and add them to the movie
    /// </summary> 
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
    
    ///<summary>
    /// Create moviedetails for a movie
    /// </summary> 
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

    ///<summary>
    /// Partially upaate a movie
    /// </summary> 
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
    
    ///<summary>
    /// Delete a movie
    /// </summary> 
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
    
    ///<summary>
    /// Get moviedetails for a movie
    /// </summary> 
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
    ///<summary>
    /// Delete a movies details
    /// </summary> 
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
    
    ///<summary>
    /// Delete all movies
    /// </summary> 
    [HttpDelete("All")]
    public async Task<IActionResult> DeleteAllMovies()
    {
        IEnumerable<Movie> movies = await repository.GetMoviesAsync();

        foreach (Movie movie in  movies)
        {
            repository.DeleteMovie(movie);
        }
        await repository.SaveChangesAsync();

        return NoContent();
    }
}
