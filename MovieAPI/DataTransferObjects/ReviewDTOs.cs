using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DataTransferObjects;

public class ReviewDTO
{
    [Required]
    public int Id { get; set; }
    public string? ReviewerName { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public int MovieId { get; set; }
}
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

public class ReviewCreateDTO
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
