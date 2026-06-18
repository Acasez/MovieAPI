namespace MovieAPI.DataTransferObjects;

public class ReviewDTO
{
    public int Id { get; set; }
    public string? ReviewerName { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public int MovieId { get; set; }
}
