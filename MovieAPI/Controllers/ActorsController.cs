using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActorsController(MovieInfoRepository repository, IMapper mapper) : ControllerBase
{
    // GET: api/Actors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActorDTO>>> GetActors()
    {
        IEnumerable<Actor> actors = await repository.GetActorsAsync();

        return Ok(mapper.Map<IEnumerable<ActorDTO>>(actors));
    }

    // GET: api/Actors/5
    [HttpGet("{actorId}")]
    public async Task<ActionResult<ActorDTO>> GetActor(int actorId)
    {
        Actor? actorEntity = await repository.GetActorAsync(actorId);

        if (actorEntity == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<ActorDTO>(actorEntity));
    }

    // PUT: api/Movies/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{actorId}")]
    public async Task<IActionResult> UpdateActor(int actorId, ActorUpdateDTO actor)
    {
        Actor? actorEntity = await repository.GetActorAsync(actorId);

        if (actorEntity == null)
        {
            return NotFound();
        }

        mapper.Map(actor, actorEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Actors
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ActorDTO>> CreateActor(ActorCreateDTO actorToCreate)
    {
        Actor? actor = mapper.Map<Actor>(actorToCreate);

        await repository.CreateActor(actor);
        await repository.SaveChangesAsync();

        ActorDTO createdActorDTO = mapper.Map<ActorDTO>(actor);
        return CreatedAtAction("GetActor", new { actorId = createdActorDTO.Id }, createdActorDTO);
    }

    [HttpPost("movies/{movieId}/actors/{actorId}")]
    public async Task<IActionResult> AddActorToMovie(int movieId, int actorId)
    {
        if (!await repository.MovieExists(movieId) || !await repository.ActorExists(actorId))
        {
            return NotFound();
        }

        MovieActor movieActor = new()
        {
            MovieId = movieId,
            ActorId = actorId
        };

        repository.AddMovieActor(movieActor);
        await repository.SaveChangesAsync();

        return Ok(movieActor);
    }

    [HttpPatch("{actorId}")]
    public async Task<ActionResult> PartiallyUpdateActor(int actorId, [FromBody] JsonPatchDocument<ActorUpdateDTO> patchDocument)
    {
        Actor? actorEntity = await repository.GetActorAsync(actorId);

        if (actorEntity == null)
        {
            return NotFound();
        }

        ActorUpdateDTO actorPatch = mapper.Map<ActorUpdateDTO>(actorEntity);

        patchDocument.ApplyTo(actorPatch, jsonPatchError =>
        {
            string key = jsonPatchError.AffectedObject.GetType().Name;
            ModelState.AddModelError(key, jsonPatchError.ErrorMessage);
        });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(actorPatch))
        {
            return BadRequest(ModelState);
        }

        mapper.Map(actorPatch, actorEntity);
        await repository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Actors/5
    [HttpDelete("{actorId}")]
    public async Task<IActionResult> DeleteActor(int actorId)
    {
        Actor? actorEntity = await repository.GetActorAsync(actorId);

        if (actorEntity == null)
        {
            return NotFound();
        }

        repository.DeleteActor(actorEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }
}
