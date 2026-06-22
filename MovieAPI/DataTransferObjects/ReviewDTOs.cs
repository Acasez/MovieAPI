using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DataTransferObjects;
// ReSharper disable once InconsistentNaming
public class ReviewDTO
{
    [Required]
    public int Id { get; set; }
    public string? ReviewerName { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public int MovieId { get; set; }
}
// ReSharper disable once InconsistentNaming
public class ReviewUpdateDTO
{
    [Required]
    public int Id { get; set; }
    public string ReviewerName { get; set; } = String.Empty;
    public string? Comment { get; set; }
    [Range(1, 10)]
    public int Rating { get; set; }
    [Required]
    public int MovieId { get; set; }
}
// ReSharper disable once InconsistentNaming
public class ReviewCreateDTO
{
    public string ReviewerName { get; set; } = String.Empty;
    public string? Comment { get; set; }
    [Range(1, 10)]
    public int Rating { get; set; }
    [Required]
    public int MovieId { get; set; }
}
