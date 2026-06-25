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
    
    private async Task<IActionResult> SeedEntities<TEntity, TCreateDto>(
        string sheetName,
        Func<IList<object>, Task<TCreateDto>> mapRowToDto,
        Func<TCreateDto, Task<IEnumerable<TEntity>>> getExistingEntities,
        Func<TCreateDto, TEntity> mapDtoToEntity, // Now synchronous
        Action<TEntity, TCreateDto> updateEntity) // Now synchronous
    {
        List<IList<object>> sheetData = await GetSheetDataAsync(SpreadsheetId, sheetName, CredentialsFilePath);

        // Skip header row if present
        if (sheetData.Count > 0 && sheetData[0][0]?.ToString() == "Id")
        {
            sheetData.RemoveAt(0);
        }

        foreach (IList<object> row in sheetData)
        {
            TCreateDto dto = await mapRowToDto(row);
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
        return await SeedEntities<Movie, MovieCreateDTO>(
            sheetName: "Movies",
            mapRowToDto: async row =>
            {
                Genre? genre = await repository.GetGenreAsync(row[4].ToString());
                Setting? setting = await repository.GetSettingAsync(row[5].ToString());

                return new MovieCreateDTO
                {
                    Title = row[1].ToString() ?? string.Empty,
                    Year = int.TryParse(row[2].ToString(), out int year) ? year : 0,
                    Duration = int.TryParse(row[3].ToString(), out int duration) ? duration : 0,
                    GenreId = genre?.Id ?? 7,
                    SettingId = setting?.Id ?? 7,
                };
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
        if (sheetData.Count > 0 && sheetData[0][0].ToString() == "Id")
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
            string moviesString = row[3].ToString() ?? string.Empty;
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
    
    [HttpPost("Seed Genres")]
    public async Task<IActionResult> SeedGenres()
    {
        return await SeedEntities<Genre, GenreCreateDTO>(
            sheetName: "Genres",
            mapRowToDto: row => Task.FromResult(new GenreCreateDTO
            {
                Name = row[1].ToString() ?? string.Empty,
                Description = row[2].ToString() ?? string.Empty,
            }),
            getExistingEntities: async dto => await repository.GetGenresAsync(),
            mapDtoToEntity: mapper.Map<Genre>,
            updateEntity: (entity, dto) => mapper.Map(dto, entity)
        );
    }
    
    [HttpPost("Seed Settings")]
    public async Task<IActionResult> SeedSettings()
    {
        return await SeedEntities<Setting, SettingCreateDTO>(
            sheetName: "Settings",
            mapRowToDto: row => Task.FromResult(new SettingCreateDTO()
            {
                Name = row[1].ToString() ?? string.Empty,
                Description = row[2].ToString() ?? string.Empty,
            }),
            getExistingEntities: async dto => await repository.GetSettingsAsync(),
            mapDtoToEntity: mapper.Map<Setting>,
            updateEntity: (entity, dto) => mapper.Map(dto, entity)
        );
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