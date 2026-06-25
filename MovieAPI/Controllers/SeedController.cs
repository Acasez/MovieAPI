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
                repository.CreateEntity(entity); // Or use your repository's Create method
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
                Year = int.TryParse(row[2]?.ToString(), out int year) ? year : 0,
                Duration = int.TryParse(row[3]?.ToString(), out int duration) ? duration : 0,
                GenreId = int.TryParse(row[4]?.ToString(), out int genreId) ? genreId : 0,
                SettingId = int.TryParse(row[5]?.ToString(), out int settingId) ? settingId : 0,
            },
            getExistingEntities: async (dto) => await repository.GetMoviesAsync(name: dto.Title, searchQuery: null),
            mapDtoToEntity: mapper.Map<Movie>,
            updateEntity: (entity, dto) => mapper.Map(dto, entity)
        );
    }
    
    [HttpPost("Seed Actors")]
    public async Task<IActionResult> SeedActors()
    {
        return await SeedEntities<Actor, ActorCreateDTO, ActorDTO>(
            sheetName: "Actors",
            mapRowToDto: row => new ActorCreateDTO
            {
                Name = row[1]?.ToString() ?? string.Empty,
                YearOfBirth = int.TryParse(row[2]?.ToString(), out int year) ? year : 0
            },
            getExistingEntities: async (dto) => await repository.GetActorsAsync(name: dto.Name, searchQuery: null),
            mapDtoToEntity: mapper.Map<Actor>,
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