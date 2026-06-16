using MovieAPI.Data;
using MovieAPI.Models;
using Microsoft.EntityFrameworkCore;

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

    internal async Task CreateMovie(Movie movie)
    {
        context.Add(movie);
    }

    internal void DeleteMovie(Movie movieEntitiy)
    {
        context.Remove(movieEntitiy);
    }

    internal async Task<bool> SaveChangesAsync()
    {
        return (await context.SaveChangesAsync() >= 0);
    }
}
