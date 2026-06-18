using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DataTransferObjects;

public class ActorDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int YearOfBirth { get; set; }
}
public class ActorCreateDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [Range(1900, 2100)]
    public int YearOfBirth { get; set; }
}

public class ActorUpdateDTO
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [Range(1900, 2100)]
    public int YearOfBirth { get; set; }
}
