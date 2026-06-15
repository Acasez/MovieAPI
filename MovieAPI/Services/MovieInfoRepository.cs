using MovieAPI.Data;
using MovieAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieAPI.Services;

public class MovieInfoRepository(MovieAPIContext context)
{
    public async Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        return await context.Movies.OrderBy(m => m.Title).ToListAsync();
    }

    internal async Task<Movie?> GetMovieAsync(int movieId)
    {
        return await context.Movies.Where(c => c.Id == movieId).FirstOrDefaultAsync();
    }
}
