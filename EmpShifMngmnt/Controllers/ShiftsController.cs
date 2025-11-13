using Application.DTOs;
using Application.UseCases.Shifts;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace EmpShifMngmnt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly CreateShiftCommand _createShiftCommand;
    private readonly GetShiftQuery _getShiftQuery;
    private readonly UpdateShiftCommand _updateShiftCommand;

    public ShiftsController(
        CreateShiftCommand createShiftCommand,
        GetShiftQuery getShiftQuery,
        UpdateShiftCommand updateShiftCommand)
    {
        _createShiftCommand = createShiftCommand;
        _getShiftQuery = getShiftQuery;
        _updateShiftCommand = updateShiftCommand;
    }

    [HttpPost]
    public async Task<ActionResult<ShiftDto>> CreateShift(
        [FromBody] CreateShiftDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _createShiftCommand.ExecuteAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetShift), new { id = result.Id }, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message, code = ex.Code });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message, code = ex.Code });
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { error = ex.Message, code = ex.Code });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftDto>> GetShift(
        long id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _getShiftQuery.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message, code = ex.Code });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftDto>>> GetShifts(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _getShiftQuery.GetAllAsync(from, to, page, 10, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ShiftDto>> UpdateShift(
        long id,
        [FromBody] UpdateShiftDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _updateShiftCommand.ExecuteAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message, code = ex.Code });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message, code = ex.Code });
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { error = ex.Message, code = ex.Code });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteShift(
        long id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _updateShiftCommand.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message, code = ex.Code });
        }
    }
}