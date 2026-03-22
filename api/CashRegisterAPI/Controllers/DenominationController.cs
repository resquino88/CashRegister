using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CashRegisterAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DenominationController(IDenominationRepository denominationRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var denominations = await denominationRepository.GetAll();
            return Ok(denominations.Select(DenominationDTO.FromEntity));
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
            var denomination = await denominationRepository.GetById(id);
            return Ok(DenominationDTO.FromEntity(denomination));
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
            var denomination = await denominationRepository.GetByName(name);
            return Ok(DenominationDTO.FromEntity(denomination));
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
