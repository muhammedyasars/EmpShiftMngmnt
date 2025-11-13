using Application.DTOs;
using Application.UseCases.Employees;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace EmpShifMngmnt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly CreateEmployeeCommand _createEmployeeCommand;
    private readonly GetEmployeeQuery _getEmployeeQuery;
    private readonly UpdateEmployeeCommand _updateEmployeeCommand;

    public EmployeesController(
        CreateEmployeeCommand createEmployeeCommand,
        GetEmployeeQuery getEmployeeQuery,
        UpdateEmployeeCommand updateEmployeeCommand)
    {
        _createEmployeeCommand = createEmployeeCommand;
        _getEmployeeQuery = getEmployeeQuery;
        _updateEmployeeCommand = updateEmployeeCommand;
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(
        [FromBody] CreateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _createEmployeeCommand.ExecuteAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetEmployee), new { id = result.Id }, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message, code = ex.Code });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(
        long id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _getEmployeeQuery.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message, code = ex.Code });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _getEmployeeQuery.GetAllActiveAsync(page, 10, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(
        long id,
        [FromBody] UpdateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _updateEmployeeCommand.ExecuteAsync(id, dto, cancellationToken);
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
    }

    [HttpPut("{id}/deactivate")]
    public async Task<ActionResult<EmployeeDto>> DeactivateEmployee(
        long id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _updateEmployeeCommand.DeactivateAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message, code = ex.Code });
        }
    }
}