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
    
    private async Task<IActionResult> SeedEntities<TEntity, TCreateDto, TSearchDto>(
        string sheetName,
        Func<IList<object>, TCreateDto> mapRowToDto,
        Func<TCreateDto, Task<IEnumerable<TEntity>>> getExistingEntities,
        Func<TCreateDto, TEntity> mapDtoToEntity,
        Action<TEntity, TCreateDto> updateEntity)
    {
        List<IList<object>> sheetData = await GetSheetDataAsync(SpreadsheetId, sheetName, CredentialsFilePath);

        // Skip header row if present
        if (sheetData.Count > 0 && sheetData[0][0]?.ToString() == "Id")
        {
            sheetData.RemoveAt(0);
        }

        foreach (TCreateDto? dto in sheetData.Select(mapRowToDto))
        {
            IEnumerable<TEntity> existingEntities = await getExistingEntities(dto);
            TEntity? existingEntity = existingEntities.FirstOrDefault();

            if (existingEntity != null)
            {
                updateEntity(existingEntity, dto);
            }
            else
            {
                TEntity entity = mapDtoToEntity(dto);
                repository.CreateEntity(entity);
            }
        }

        await repository.SaveChangesAsync();
        return Ok($"{sheetName} seeded successfully!");
    }
    
    [HttpPost("Seed Movies")]
    public async Task<IActionResult> SeedMovies()
    {
        return await SeedEntities<Movie, MovieCreateDTO, MovieDTO>(
            sheetName: "Movies",
            mapRowToDto: row => new MovieCreateDTO
            {
                Title = row[1]?.ToString() ?? string.Empty,
                Year = int.TryParse(row[2].ToString(), out int year) ? year : 0,
                Duration = int.TryParse(row[3]?.ToString(), out int duration) ? duration : 0,
                GenreId = int.TryParse(row[4]?.ToString(), out int genreId) ? genreId : 0,
                SettingId = int.TryParse(row[5]?.ToString(), out int settingId) ? settingId : 0,
            },
            getExistingEntities: async dto => await repository.GetMoviesAsync(name: dto.Title, searchQuery: null),
            mapDtoToEntity: mapper.Map<Movie>,
            updateEntity: (entity, dto) => mapper.Map(dto, entity)
        );
    }
    
    [HttpPost("Seed Actors")]
    public async Task<IActionResult> SeedActors()
    {
        List<IList<object>> sheetData = await GetSheetDataAsync(SpreadsheetId, "Actors", CredentialsFilePath);

        // Skip header row if present
        if (sheetData.Count > 0 && sheetData[0][0]?.ToString() == "Id")
        {
            sheetData.RemoveAt(0);
        }

        foreach (IList<object> row in sheetData)
        {
            // Map the row to ActorCreateDTO
            ActorCreateDTO actorToCreate = new()
            {
                Name = row[1].ToString() ?? string.Empty,
                YearOfBirth = int.TryParse(row[2].ToString(), out int year) ? year : 0
            };

            // Check if the actor already exists
            IEnumerable<Actor> existingActors = await repository.GetActorsAsync(name: actorToCreate.Name, searchQuery: null);
            Actor? existingActor = existingActors.FirstOrDefault();

            // Create or update the actor
            Actor actor;
            if (existingActor != null)
            {
                mapper.Map(actorToCreate, existingActor);
                actor = existingActor;
            }
            else
            {
                actor = mapper.Map<Actor>(actorToCreate);
                await repository.CreateActor(actor);
            }

            // Handle the Movies column (e.g., "Rogue One, Dead Man's Chest")
            string? moviesString = row[3].ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(moviesString)) continue;
            // Split the movies string by commas and trim whitespace
            List<string> movieTitles = moviesString.Split(',')
                .Select(title => title.Trim())
                .Where(title => !string.IsNullOrEmpty(title))
                .ToList();

            // For each movie title, find the movie and link the actor
            foreach (string movieTitle in movieTitles)
            {
                IEnumerable<Movie> movies = await repository.GetMoviesAsync(name: movieTitle, searchQuery: null);
                Movie? movie = movies.FirstOrDefault();

                if (movie == null) continue;
                // Add the actor to the movie's Actors collection
                movie.Actors ??= new List<Actor>();
                if (!movie.Actors.Contains(actor))
                {
                    movie.Actors.Add(actor);
                }
            }
        }

        await repository.SaveChangesAsync();
        return Ok("Actors and their movies seeded successfully!");
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