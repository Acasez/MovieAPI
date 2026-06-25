using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SettingsController(MovieInfoRepository repository, IMapper mapper) : ControllerBase
{
    // GET: api/Settings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SettingDTO>>> GetSettings()
    {
        IEnumerable<Setting> settings = await repository.GetSettingsAsync();

        return Ok(mapper.Map<IEnumerable<SettingDTO>>(settings));
    }

    // GET: api/Settings/5
    [HttpGet("{settingId:int}")]
    public async Task<ActionResult<SettingDTO>> GetSetting(int settingId)
    {
        Setting? settingEntity = await repository.GetSettingAsync(settingId);

        if (settingEntity == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<SettingDTO>(settingEntity));
    }

    // PUT: api/Movies/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{settingId:int}")]
    public async Task<IActionResult> UpdateSetting(int settingId, SettingUpdateDTO setting)
    {
        Setting? settingEntity = await repository.GetSettingAsync(settingId);

        if (settingEntity == null)
        {
            return NotFound();
        }

        mapper.Map(setting, settingEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Settings
    [HttpPost]
    public async Task<ActionResult<SettingDTO>> CreateSetting(SettingCreateDTO settingToCreate)
    {
        Setting? setting = mapper.Map<Setting>(settingToCreate);

        await repository.CreateSetting(setting);
        await repository.SaveChangesAsync();

        SettingDTO createdSettingDto = mapper.Map<SettingDTO>(setting);
        return CreatedAtAction("GetSetting", new { settingId = createdSettingDto.Id }, createdSettingDto);
    }

    [HttpPatch("{settingId:int}")]
    public async Task<ActionResult> PartiallyUpdateSetting(int settingId, [FromBody] JsonPatchDocument<SettingUpdateDTO> patchDocument)
    {
        Setting? settingEntity = await repository.GetSettingAsync(settingId);

        if (settingEntity == null)
        {
            return NotFound();
        }

        SettingUpdateDTO settingPatch = mapper.Map<SettingUpdateDTO>(settingEntity);

        patchDocument.ApplyTo(settingPatch, jsonPatchError =>
        {
            string key = jsonPatchError.AffectedObject.GetType().Name;
            ModelState.AddModelError(key, jsonPatchError.ErrorMessage);
        });

        if (!ModelState.IsValid || !TryValidateModel(settingPatch))
        {
            return BadRequest(ModelState);
        }

        mapper.Map(settingPatch, settingEntity);
        await repository.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Settings/5
    [HttpDelete("{settingId:int}")]
    public async Task<IActionResult> DeleteSetting(int settingId)
    {
        Setting? settingEntity = await repository.GetSettingAsync(settingId);

        if (settingEntity == null)
        {
            return NotFound();
        }

        repository.DeleteSetting(settingEntity);
        await repository.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete("All")]
    public async Task<IActionResult> DeleteAllSettings()
    {
        IEnumerable<Setting> settings = await repository.GetSettingsAsync();

        foreach (Setting setting in settings)
        {
            repository.DeleteSetting(setting);
        }
        await repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{movieId:int}/settings")]
    public async Task<IActionResult> AddSettingsToMovie(int movieId, List<int> settingIds)
    {
        Movie? movieEntity = await repository.GetMovieAsync(movieId);
        if (movieEntity == null)
        {
            return NotFound();
        }

        foreach (int settingId in settingIds)
        {
            Setting? setting = await repository.GetSettingAsync(settingId);
            if (setting != null)
            {
                //MovieInfoRepository.SetMovieSetting(movieEntity, setting);
            }
        }

        await repository.SaveChangesAsync();
        return Ok(movieEntity);
    }
}
