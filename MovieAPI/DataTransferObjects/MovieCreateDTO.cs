using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DataTransferObjects;

public class MovieCreateDTO
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int? Year { get; set; }

    public string? Genre { get; set; }

    [Range(1, 1000)] 
    public int? Duration { get; set; }

    public List<int>? ActorIds { get; set; }
}
