using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;
using System.Numerics;

namespace MovieAPI.Services;

public class MovieInfoRepository(MovieAPIContext context)
{
    public async Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        return await context.Movie.OrderBy(m => m.Title).ToListAsync();
    }

    internal async Task<Movie?> GetMovieAsync(int movieId)
    {
        return await context.Movie.Where(m => m.Id == movieId).FirstOrDefaultAsync();
    }

    internal async Task CreateMovie(Movie movie, MovieCreateDTO movieToCreate)
    {
        context.Add(movie);

        if (movieToCreate.ActorIds != null && movieToCreate.ActorIds.Count != 0)
        {
            foreach (int actorId in movieToCreate.ActorIds)
            {
                Actor? actor = await context.Actor.FindAsync(actorId);
                if (actor != null)
                {
                    movie.Actors?.Add(actor);
                }
            }
        }
    }

    internal void DeleteMovie(Movie movieEntitiy)
    {
        context.Remove(movieEntitiy);
    }

    internal async Task<bool> SaveChangesAsync()
    {
        return (await context.SaveChangesAsync() >= 0);
    }


    internal async Task<Actor?> GetActorAsync(int actorId)
    {
        return await context.Actor.Where(a => a.Id == actorId).FirstOrDefaultAsync();
    }

    internal async Task CreateActor(Actor actor, int movieId = 0)
    {
        context.Add(actor);
    }

    internal void DeleteActor(Actor actorEntity)
    {
        context.Remove(actorEntity);
    }
    
    public async Task<IEnumerable<Actor>> GetActorsAsync()
    {
        return await context.Actor.OrderBy(a => a.Name).ToListAsync();
    }
    internal async Task<bool> MovieExists(int movieID)
    {
        return await context.Movie.AnyAsync(m => m.Id == movieID);
    }

    internal async Task<bool> ActorExists(int actorId)
    {
        return await context.Actor.AnyAsync(a => a.Id == actorId);
    }

    internal static void AddActorToMovie(Movie movieEntity, Actor actor)
    {
        if (movieEntity.Actors == null)
        {
            return;
        }
        if (!movieEntity.Actors.Contains(actor))
        {
            movieEntity.Actors?.Add(actor);
        }
        
    }
}
