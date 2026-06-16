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
        return await context.Actor.Where(a => a.Id == actorId).FirstOrDefaultAsync();
    }

    internal async Task CreateActor(Actor actor)
    {
        context.Add(actor);
    }

    internal void DeleteActor(Actor actorEntity)
    {
        context.Remove(actorEntity);
    }

    internal async Task<bool> SaveChangesAsync()
    {
        return (await context.SaveChangesAsync() >= 0);
    }
}