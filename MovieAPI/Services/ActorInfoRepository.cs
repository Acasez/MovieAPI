using MovieAPI.Data;
using MovieAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieAPI.Services;

public class ActorInfoRepository(ActorContext context)
{
    public async Task<IEnumerable<Actor>> GetActorsAsync()
    {
        return await context.Actor.OrderBy(a => a.Name).ToListAsync();
    }

    internal async Task<Actor?> GetActorAsync(int actorId)
    {
        return await context.Actor.Where(c => c.Id == actorId).FirstOrDefaultAsync();
    }

    internal async Task CreateMovie(Actor actor)
    {
        context.Add(actor);
    }

    internal void DeleteMovie(Actor actorEntity)
    {
        context.Remove(actorEntity);
    }

    internal async Task<bool> SaveChangesAsync()
    {
        return (await context.SaveChangesAsync() >= 0);
    }
}