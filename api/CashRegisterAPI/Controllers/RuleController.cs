using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CashRegisterAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RuleController(IRuleRepository ruleRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var rules = await ruleRepository.GetAll();
            return Ok(rules.Select(RuleDTO.FromEntity));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveRules()
    {
        try
        {
            var rules = await ruleRepository.GetActiveRules();
            return Ok(rules.Select(RuleDTO.FromEntity));
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
            var rule = await ruleRepository.GetById(id);
            return Ok(RuleDTO.FromEntity(rule));
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
            var rule = await ruleRepository.GetByName(name);
            return Ok(RuleDTO.FromEntity(rule));
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
