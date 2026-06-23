using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models;

public class Genre
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    public ICollection<Movie> Movies { get; set; } = [];
}