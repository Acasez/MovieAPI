using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DataTransferObjects;
using MovieAPI.Interfaces;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenresController(IMovieService repository, IMapper mapper) : ControllerBase
{
    ///<summary>
    /// Get all genres
    /// </summary> 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDTO>>> GetGenres()
    {
        IEnumerable<Genre> genres = await repository.GetGenresAsync();

        return Ok(mapper.Map<IEnumerable<GenreDTO>>(genres));
    }
    
    ///<summary>
    /// Get a genre
    /// </summary> 
    [HttpGet("{genreId:int}")]
    public async Task<ActionResult<GenreDTO>> GetGenre(int genreId)
    {
        Genre? genreEntity = await repository.GetGenreAsync(genreId);

        if (genreEntity == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<GenreDTO>(genreEntity));
    }
    
    ///<summary>
    /// Get a genre by name
    /// </summary> 
    [HttpGet("{genreName}")]
    public async Task<ActionResult<GenreDTO>> GetGenreByName(string genreName)
    {
        Genre? genreEntity = await repository.GetGenreAsync(genreName);

        if (genreEntity == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<GenreDTO>(genreEntity));
    }
    
    ///<summary>
    /// Update a genre
    /// </summary> 
    [HttpPut("{genreId:int}")]
    public async Task<IActionResult> UpdateGenre(int genreId, GenreUpdateDTO genre)
    {
        Genre? genreEntity = await repository.GetGenreAsync(genreId);

        if (genreEntity == null)
        {
            return NotFound();
        }

        mapper.Map(genre, genreEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }
    
    ///<summary>
    /// Create a genre
    /// </summary> 
    [HttpPost]
    public async Task<ActionResult<GenreDTO>> CreateGenre(GenreCreateDTO genreToCreate)
    {
        Genre? genre = mapper.Map<Genre>(genreToCreate);

        await repository.CreateGenre(genre);
        await repository.SaveChangesAsync();

        GenreDTO createdGenreDto = mapper.Map<GenreDTO>(genre);
        return CreatedAtAction("GetGenre", new { genreId = createdGenreDto.Id }, createdGenreDto);
    }
    
    ///<summary>
    /// Partially update a genre
    /// </summary> 
    [HttpPatch("{genreId:int}")]
    public async Task<ActionResult> PartiallyUpdateGenre(int genreId, [FromBody] JsonPatchDocument<GenreUpdateDTO> patchDocument)
    {
        Genre? genreEntity = await repository.GetGenreAsync(genreId);

        if (genreEntity == null)
        {
            return NotFound();
        }

        GenreUpdateDTO genrePatch = mapper.Map<GenreUpdateDTO>(genreEntity);

        patchDocument.ApplyTo(genrePatch, jsonPatchError =>
        {
            string key = jsonPatchError.AffectedObject.GetType().Name;
            ModelState.AddModelError(key, jsonPatchError.ErrorMessage);
        });

        if (!ModelState.IsValid || !TryValidateModel(genrePatch))
        {
            return BadRequest(ModelState);
        }

        mapper.Map(genrePatch, genreEntity);
        await repository.SaveChangesAsync();
        return NoContent();
    }
    
    ///<summary>
    /// Delete a genre
    /// </summary> 
    [HttpDelete("{genreId:int}")]
    public async Task<IActionResult> DeleteGenre(int genreId)
    {
        Genre? genreEntity = await repository.GetGenreAsync(genreId);

        if (genreEntity == null)
        {
            return NotFound();
        }

        repository.DeleteGenre(genreEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }
    
    ///<summary>
    /// Delete all genres
    /// </summary> 
    [HttpDelete("All")]
    public async Task<IActionResult> DeleteAllGenres()
    {
        IEnumerable<Genre> genres = await repository.GetGenresAsync();

        foreach (Genre genre in genres)
        {
            repository.DeleteGenre(genre);
        }
        await repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{movieId:int}/genres")] //Not done 
    public async Task<IActionResult> AddGenresToMovie(int movieId, List<int> genreIds)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);
        if (movieEntity == null)
        {
            return NotFound();
        }

        foreach (int genreId in genreIds)
        {
            Genre? genre = await repository.GetGenreAsync(genreId);
            if (genre != null)
            {
                //MovieInfoRepository.SetMovieGenre(movieEntity, genre);
            }
        }

        await repository.SaveChangesAsync();
        return Ok(movieEntity);
    }
}
