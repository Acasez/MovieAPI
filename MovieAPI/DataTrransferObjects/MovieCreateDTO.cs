namespace MovieAPI.DataTrransferObjects;

public class MovieCreateDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
    public int? Duration { get; set; }
}
