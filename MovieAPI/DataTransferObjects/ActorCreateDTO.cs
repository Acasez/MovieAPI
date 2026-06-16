using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DataTransferObjects;

public class ActorCreateDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    public int YearOfBirth { get; set; }
}
