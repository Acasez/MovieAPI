using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DataTrransferObjects;

public class MovieCreateDTO
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;
    [Range(1900, 2100)]
    public int? Year { get; set; }
    public string? Genre { get; set; }
    [Range(1, 1000)] //In minutes
    public int? Duration { get; set; }
}
