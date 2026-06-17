using MovieAPI.Data;
using MovieAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieAPI.Services;

public class ActorInfoRepository(MovieAPIContext context)
{
    public async Task<IEnumerable<Actor>> GetActorsAsync()
    {
        return await context.Actor.OrderBy(a => a.Name).ToListAsync();
    }

    internal async Task<Actor?> GetActorAsync(int actorId)
    {
        return await context.Actor.Where(a => a.Id == actorId).FirstOrDefaultAsync();
    }

    internal async Task CreateActor(Actor actor, int movieId = 0)
    {
        context.Add(actor);
        if (movieId != 0)
        {
            CreateMovieActor(actor.Id, movieId);
        }
    }

    internal async Task CreateMovieActor(int actorId, int movieid)
    {
        MovieActor newNovieActor = new MovieActor();
        context.Add(newNovieActor);
    }

    internal void DeleteActor(Actor actorEntity)
    {
        context.Remove(actorEntity);
    }

    internal async Task<bool> SaveChangesAsync()
    {
        return (await context.SaveChangesAsync() >= 0);
    }

    internal async Task<bool> MovieExists(int movieID)
    {
        return await context.Movie.AnyAsync(a => a.Id == movieID);
    }
}