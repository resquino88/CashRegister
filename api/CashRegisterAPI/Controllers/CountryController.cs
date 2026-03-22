using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CashRegisterAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CountryController(ICountryRepository countryRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var countries = await countryRepository.GetAll();
            return Ok(countries.Select(CountryDTO.FromEntity));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var country = await countryRepository.GetById(id);
            return Ok(CountryDTO.FromEntity(country));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        try
        {
            var country = await countryRepository.GetByName(name);
            return Ok(CountryDTO.FromEntity(country));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
