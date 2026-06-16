using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DataTransferObjects;

public class ActorUpdateDTO
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    public int YearOfBirth { get; set; }
}
