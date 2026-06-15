namespace MovieAPI.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string Genre { get; set; }
    //In minutes
    public int Duration { get; set; }

}
