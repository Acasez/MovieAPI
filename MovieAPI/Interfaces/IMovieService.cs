using MovieAPI.DataTransferObjects;
using MovieAPI.Models;

namespace MovieAPI.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<Movie>> GetMoviesAsync(string? name = "", string? searchQuery = "");
    Task<Movie?> GetMovieAsync(int movieId);
    Task CreateMovie(Movie movie, MovieCreateDTO movieToCreate);
    void DeleteMovie(Movie movieEntitiy);
    Task SaveChangesAsync();
    Task CreateActor(Actor actor, Movie? movie = null);
    void DeleteActor(Actor actorEntity);
    Task<IEnumerable<Actor>> GetActorsAsync(string? name = "", string? searchQuery = "");
    Task<Actor?> GetActorAsync(int actorId);
    Task<bool> MovieExists(int movieId);
    Task<bool> ActorExists(int actorId);
    Task<IEnumerable<Review>> GetReviewsAsync();
    Task<Review?> GetReviewAsync(int reviewId);
    Task CreateReview(Review review);
    void DeleteReview(Review reviewEntity);
    Task<IEnumerable<Genre>> GetGenresAsync();
    Task<Genre?> GetGenreAsync(int genreId);
    Task<Genre?> GetGenreAsync(string? genreName);
    Task CreateGenre(Genre genre);
    void DeleteGenre(Genre genreEntity);
    void CreateMovieDetails(MovieDetails details, Movie movieEntity);
    Task<IEnumerable<MovieDetails>> GetMovieDetailsAsync();
    Task<MovieDetails?> GetMovieDetailAsync(int movieDetailsId);
    Task<MovieDetails?> GetMovieDetailByMovieIdAsync(int movieId);
    Task<IEnumerable<Setting>> GetSettingsAsync();
    Task<Setting?> GetSettingAsync(int settingId);
    Task<Setting?> GetSettingAsync(string? settingName);
    Task CreateSetting(Setting setting);
    void DeleteSetting(Setting setting);
    void DeleteMovieDetails(MovieDetails movieDetails);
    Task<bool> MovieExistsAsync(int movieId);
    void CreateEntity<TEntity>(TEntity entity);
    Task ResetAllTables();
}