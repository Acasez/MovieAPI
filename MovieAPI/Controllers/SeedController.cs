using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DataTransferObjects;
using MovieAPI.Services;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController(MovieInfoRepository repository, IMapper mapper) : ControllerBase
{
    private const string SpreadsheetId = "11s2lLlNAWnHhhpyIxO4nm5htyaNtvoQYOgvivIzggyw";
    private const string CredentialsFilePath = "C:/Users/Edvin/Documents/GoogleAPI/GoogleAPI.json";

    [HttpPost("Seed Movies")]
    public async Task<IActionResult> SeedMovies()
    {
        List<IList<object>> sheetData = await GetSheetDataAsync(SpreadsheetId, "Movies", CredentialsFilePath);

        // Skip header row if present
        if (sheetData.Count > 0 && sheetData[0][0]?.ToString() == "Id")
        {
            sheetData.RemoveAt(0);
        }

        foreach (IList<object> row in sheetData)
        {
            int movieId = int.Parse(row[0]?.ToString() ?? "0");
            
            // Map the row data to your MovieCreateDTO
            MovieCreateDTO movieToCreate = new()
            {
                Title = row[1]?.ToString() ?? string.Empty,
                Year = int.TryParse(row[2]?.ToString(), out int year) ? year : 0,
                Duration = int.TryParse(row[3]?.ToString(), out int duration) ? duration : 0,
                GenreId = int.TryParse(row[4]?.ToString(), out int genreId) ? genreId : 0,
                SettingId = int.TryParse(row[5]?.ToString(), out int settingId) ? settingId : 0,
            };

            // Use your existing CreateMovie logic
            Movie? existingMovie = await repository.GetMovieAsync(movieId);    
            if (existingMovie != null)
            {
                // Update existing movie
                mapper.Map(movieToCreate, existingMovie);
            }
            else
            {
                // Insert new movie
                Movie? movie = mapper.Map<Movie>(movieToCreate);
                await repository.CreateMovie(movie, movieToCreate);
            }
        }

        await repository.SaveChangesAsync();
        return Ok("Movies seeded successfully!");
    }

    private static async Task<List<IList<object>>> GetSheetDataAsync(string spreadsheetId, string sheetName, string credentialsFilePath)
    {
        GoogleCredential? credential = GoogleCredential.FromFile(credentialsFilePath)
            .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

        SheetsService service = new(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Sheets API C#"
        });

        string range = $"{sheetName}";
        SpreadsheetsResource.ValuesResource.GetRequest? request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        ValueRange? response = await request.ExecuteAsync();
        return (List<IList<object>>)response.Values ?? [];
    }
}