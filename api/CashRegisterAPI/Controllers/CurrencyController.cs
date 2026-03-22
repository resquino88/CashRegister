using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CashRegisterAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CurrencyController(ICurrencyRepository currencyRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var currencies = await currencyRepository.GetAll();
            return Ok(currencies.Select(CurrencyDTO.FromEntity));
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
            var currency = await currencyRepository.GetById(id);
            return Ok(CurrencyDTO.FromEntity(currency));
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
            var currency = await currencyRepository.GetByName(name);
            return Ok(CurrencyDTO.FromEntity(currency));
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
