using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DataTransferObjects;
// ReSharper disable once InconsistentNaming
public class GenreDTO
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    public string Description { get; set; }
    public  ICollection<MovieDTO> Movies { get; set; } = [];
}
// ReSharper disable once InconsistentNaming
public class GenreCreateDTO
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    public string Description { get; set; }
    public  ICollection<MovieDTO> Movies { get; set; } = [];
}
// ReSharper disable once InconsistentNaming
public class GenreUpdateDTO
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }
}
