using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DataTransferObjects;
using MovieAPI.Interfaces;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActorsController(IMovieService repository, IMapper mapper) : ControllerBase
{
    ///<summary>
    /// Get all actors
    /// </summary> 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActorDTO>>> GetActors(string? name, string? searchQuery)
    {
        IEnumerable<Actor> actors = await repository.GetActorsAsync(name, searchQuery);

        return Ok(mapper.Map<IEnumerable<ActorDTO>>(actors));
    }
    
    ///<summary>
    /// Get a actor
    /// </summary> 
    [HttpGet("{actorId:int}")]
    public async Task<ActionResult<ActorDTO>> GetActor(int actorId)
    {
        Actor? actorEntity = await repository.GetActorAsync(actorId);

        if (actorEntity == null)
        {
            return NotFound("Actor not found");
        }

        return Ok(mapper.Map<ActorDTO>(actorEntity));
    }
    
    ///<summary>
    /// Get a actor by name
    /// </summary> 
    [HttpGet("{actorName}")]
    public async Task<ActionResult<MovieDTO>> GetActor(string actorName)
    {
         Actor? actorEntity = await repository.GetActorAsync(actorName);

         if (actorEntity == null)
         {
             return NotFound("Actor not found");
         }

         return Ok(mapper.Map<ActorDTO>(actorEntity));
    }
    
    ///<summary>
    /// Get all movies an actor has been in
    /// </summary> 
    [HttpGet("{actorId:int}/movies")]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMoviesFromActor(int actorId)
    {
        Actor? actorEntity = await repository.GetActorAsync(actorId);

        if (actorEntity == null)
        {
            return NotFound("Actor not found");
        }
        
        IEnumerable<Movie>? movies = actorEntity.Movies;
        if (movies == null || !movies.Any())
        {
            return NotFound("Actor has no movies listed");
        }

        return Ok(mapper.Map<IEnumerable<MovieDTO>>(movies));
    }
    
    ///<summary>
    /// Update an actor
    /// </summary> 
    [HttpPut("{actorId:int}")]
    public async Task<IActionResult> UpdateActor(int actorId, ActorUpdateDTO actor)
    {
        Actor? actorEntity = await repository.GetActorAsync(actorId);

        if (actorEntity == null)
        {
            return NotFound("Actor not found");
        }

        mapper.Map(actor, actorEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }

    ///<summary>
    /// Add an actor
    /// </summary> 
    [HttpPost]
    public async Task<ActionResult<ActorDTO>> CreateActor(ActorCreateDTO actorToCreate)
    {
        Actor? actor = mapper.Map<Actor>(actorToCreate);

        await repository.CreateActor(actor);
        await repository.SaveChangesAsync();

        ActorDTO createdActorDto = mapper.Map<ActorDTO>(actor);
        return CreatedAtAction("GetActor", new { actorId = createdActorDto.Id }, createdActorDto);
    }
    
    ///<summary>
    /// Partially update an actor
    /// </summary> 
    [HttpPatch("{actorId:int}")]
    public async Task<ActionResult> PartiallyUpdateActor(int actorId, [FromBody] JsonPatchDocument<ActorUpdateDTO> patchDocument)
    {
        Actor? actorEntity = await repository.GetActorAsync(actorId);

        if (actorEntity == null)
        {
            return NotFound("Actor not found");
        }

        ActorUpdateDTO actorPatch = mapper.Map<ActorUpdateDTO>(actorEntity);

        patchDocument.ApplyTo(actorPatch, jsonPatchError =>
        {
            string key = jsonPatchError.AffectedObject.GetType().Name;
            ModelState.AddModelError(key, jsonPatchError.ErrorMessage);
        });

        if (!ModelState.IsValid || !TryValidateModel(actorPatch))
        {
            return BadRequest(ModelState);
        }

        mapper.Map(actorPatch, actorEntity);
        await repository.SaveChangesAsync();
        return NoContent();
    }

    ///<summary>
    /// Delete an actor
    /// </summary> 
    [HttpDelete("{actorId:int}")]
    public async Task<IActionResult> DeleteActor(int actorId)
    {
        Actor? actorEntity = await repository.GetActorAsync(actorId);

        if (actorEntity == null)
        {
            return NotFound("Actor not found");
        }

        repository.DeleteActor(actorEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }
    
    ///<summary>
    /// Delete all actors
    /// </summary> 
    [HttpDelete("All")]
    public async Task<IActionResult> DeleteAllActors()
    {
        IEnumerable<Actor> actors = await repository.GetActorsAsync();

        foreach (Actor actor in actors)
        {
            repository.DeleteActor(actor);
        }
        await repository.SaveChangesAsync();

        return NoContent();
    }
    
    ///<summary>
    /// Add a number of actors to a movie
    /// </summary> 
    [HttpPost("{movieId:int}/actors")]
    public async Task<IActionResult> AddActorsToMovie(int movieId, List<int> actorIds)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);
        if (movieEntity == null)
        {
            return NotFound();
        }

        foreach (int actorId in actorIds)
        {
            Actor? actor = await repository.GetActorAsync(actorId);
            if (actor != null)
            {
                MovieInfoRepository.AddActorToMovie(movieEntity, actor);
            }
        }

        await repository.SaveChangesAsync();
        return Ok(movieEntity);
    }
}
