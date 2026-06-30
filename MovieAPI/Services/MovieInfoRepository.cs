using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.DataTransferObjects;
using MovieAPI.Interfaces;
using MovieAPI.Models;
namespace MovieAPI.Services;

public class MovieInfoRepository(MovieAPIContext context) : IMovieService
{
    public async Task<IEnumerable<Movie>> GetMoviesAsync(string? name = "", string? searchQuery = "")
    {
        IQueryable<Movie> collection = context.Movie;

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            collection = collection.Where(c => c.Title == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(c => c.Title.Contains(searchQuery));
            //TODO, re add search by genre
        }

        return await collection.Include(m => m.MovieDetails) // Load MovieDetails
            .Where(m => string.IsNullOrEmpty(name) || m.Title.Contains(name))
            .ToListAsync();;
    }

    public async Task<Movie?> GetMovieAsync(int movieId)
    {
        return await context.Movie.Where(m => m.Id == movieId).FirstOrDefaultAsync();
    }

    public async Task<Movie?> GetMovieAsync(string movieTitle)
    {
        return await context.Movie.Where(m => m.Title == movieTitle).FirstOrDefaultAsync();
    }

    public async Task CreateMovie(Movie movie, MovieCreateDTO movieToCreate)
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

    public void DeleteMovie(Movie movieEntitiy)
    {
        context.Remove(movieEntitiy);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public Task CreateActor(Actor actor, Movie? movie = null)
    {
        try
        {
            if (movie != null)
            {
                actor.Movies?.Add(movie);
                AddActorToMovie(movie, actor);
            }
            context.Add(actor);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    public void DeleteActor(Actor actorEntity)
    {
        context.Remove(actorEntity);
    }
    
    public async Task<IEnumerable<Actor>> GetActorsAsync(string? name = "", string? searchQuery = "")
    {
        IQueryable<Actor> collection = context.Actor;

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            collection = collection.Where(c => c.Name == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(a => a.Name != null && a.Name.Contains(searchQuery));
        }

        return await collection.OrderBy(a => a.Name).ToListAsync();
    }

    public async Task<Actor?> GetActorAsync(int actorId)
    {
        return await context.Actor.Where(a => a.Id == actorId).
            Include(a => a.Movies).FirstOrDefaultAsync();
    }

    public async Task<bool> MovieExists(int movieId)
    {
        return await context.Movie.AnyAsync((m => m.Id == movieId));
    }

    public async Task<bool> ActorExists(int actorId)
    {
        return await context.Actor.AnyAsync((a => a.Id == actorId));
    }

    internal static bool AddActorToMovie(Movie movieEntity, Actor actor)
    {
        if (movieEntity.Actors == null)
        {
            return false;
        }
        if (!movieEntity.Actors.Contains(actor))
        {
            movieEntity.Actors?.Add(actor);
        }
        return true;
    }

    public async Task<IEnumerable<Review>> GetReviewsAsync()
    {
        return await context.Review.OrderBy(r => r.ReviewerName).ToListAsync();
    }

    public async Task<Review?> GetReviewAsync(int reviewId)
    {
        return await context.Review.Where(r => r.Id == reviewId).FirstOrDefaultAsync();
    }

    public async Task CreateReview(Review review)
    {
        context.Add(review);
    }

    public void DeleteReview(Review reviewEntity)
    {
        context.Remove(reviewEntity);
    }
    
    public async Task<IEnumerable<Genre>> GetGenresAsync()
    {
        return await context.Genre.OrderBy(g => g.Name).ToListAsync();
    }

    public async Task<Genre?> GetGenreAsync(int genreId)
    {
        return await context.Genre.Where(g => g.Id == genreId).FirstOrDefaultAsync();
    }
    
    public async Task<Genre?> GetGenreAsync(string? genreName)
    {
        return await context.Genre.Where(g => g.Name == genreName).FirstOrDefaultAsync();
    }

    public Task CreateGenre(Genre genre)
    {
        context.Add(genre);
        return Task.CompletedTask;
    }

    public void DeleteGenre(Genre genreEntity)
    {
        context.Remove(genreEntity);
    }

    public void CreateMovieDetails(MovieDetails details, Movie movieEntity)
    {
        movieEntity.MovieDetails = details;  // EF auto-sets MovieDetailsId
        details.MovieId = movieEntity.Id;
        context.Add(details);
    }
    
    public async Task<IEnumerable<MovieDetails>> GetMovieDetailsAsync()
    {
        return await context.MovieDetails.OrderBy(md => md.MovieId).ToListAsync();
    }

    public async Task<MovieDetails?> GetMovieDetailAsync(int movieDetailsId)
    {
        return await context.MovieDetails.Where(md => md.Id == movieDetailsId).FirstOrDefaultAsync();
    }
    public async Task<MovieDetails?> GetMovieDetailByMovieIdAsync(int movieId)
    {
        return await context.MovieDetails.Where(md => md.MovieId == movieId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Setting>> GetSettingsAsync()
    {
        return await context.Settings.OrderBy(s => s.Name).ToListAsync();
    }
    
    public async Task<Setting?> GetSettingAsync(int settingId)
    {
        return await context.Settings.Where(s => s.Id == settingId).FirstOrDefaultAsync();
    }
    
    public async Task<Setting?> GetSettingAsync(string? settingName)
    {
        return await context.Settings.Where(g => g.Name == settingName).FirstOrDefaultAsync();
    }

    public async Task CreateSetting(Setting setting)
    {
        context.Add(setting);
    }
    public void DeleteSetting(Setting setting)
    {
        context.Remove(setting);
    }

    public void DeleteMovieDetails(MovieDetails movieDetails)
    {
        context.Remove(movieDetails);
    }

    public async Task<bool> MovieExistsAsync(int movieId)
    {
        return await context.Movie.AnyAsync(m => m.Id == movieId);
    }

    public void CreateEntity<TEntity>(TEntity entity)
    {
        if (entity != null) context.Add(entity);
    }

    public async Task ResetAllTables()
    {
        await context.MovieDetails.ExecuteDeleteAsync();
        await context.Actor.ExecuteDeleteAsync();
        await context.Movie.ExecuteDeleteAsync();
        await context.Genre.ExecuteDeleteAsync();
        await context.Settings.ExecuteDeleteAsync();
        await context.SaveChangesAsync();
    }
}
