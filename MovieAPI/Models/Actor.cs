using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Models;

public class Actor
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int YearOfBirth { get; set; }
    public ICollection<Movie>? Movies { get; set; }
}

public class MovieActor
{
    public int MovieId { get; set; }
    public int ActorID { get; set; }
}
