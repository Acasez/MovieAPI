using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DataTransferObjects;
// ReSharper disable once InconsistentNaming
public class MovieDetailsDTO
{
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string Synopsis { get; set; }
    [Required]
    [MaxLength(30)]
    public string Language { get; set; }
    public float Budget { get; set; }
}
// ReSharper disable once InconsistentNaming
public class MovieDetailsCreateDTO
{
    [Required]
    [MaxLength(200)]
    public string Synopsis { get; set; }
    [Required]
    [MaxLength(30)]
    public string Language { get; set; }
    public float Budget { get; set; }
}
// ReSharper disable once InconsistentNaming
public class MovieDetailsUpdateDTO
{
    [Required]
    [MaxLength(200)]
    public string Synopsis { get; set; }
    [Required]
    [MaxLength(30)]
    public string Language { get; set; }
    public float Budget { get; set; }
}
