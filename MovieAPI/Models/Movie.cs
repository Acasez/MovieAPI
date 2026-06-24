namespace MovieAPI.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    //In minutes
    public int Duration { get; set; }
    //public int MovieDetailsId { get; set; }
    public MovieDetails? MovieDetails { get; set; }
    // Foreign key and navigation property
    public int GenreId { get; set; }
    public int Setting { get; set; }
    public ICollection<Actor>? Actors { get; set; } = [];
    public ICollection<Review>? Reviews { get; set; } = [];
}
