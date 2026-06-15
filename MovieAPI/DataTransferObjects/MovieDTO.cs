namespace MovieAPI.DataTransferObjects;

public class MovieDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string Genre { get; set; }
    public int Duration { get; set; }

}