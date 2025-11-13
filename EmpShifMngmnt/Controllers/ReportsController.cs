using Application.DTOs;
using Application.UseCases.Reports;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace EmpShifMngmnt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportQueries _reportQueries;

    public ReportsController(ReportQueries reportQueries)
    {
        _reportQueries = reportQueries;
    }

    [HttpGet("employees/{id}/hours")]
    public async Task<ActionResult<WorkingHoursDto>> GetEmployeeHours(
        long id,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _reportQueries.GetEmployeeHoursAsync(id, from, to, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message, code = ex.Code });
        }
    }

    [HttpGet("most-hours")]
    public async Task<ActionResult<MostHoursReportDto>> GetEmployeeWithMostHours(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportQueries.GetEmployeeWithMostHoursAsync(month, year, cancellationToken);
        return Ok(result);
    }

    [HttpGet("average-hours")]
    public async Task<ActionResult<WorkingHoursDto>> GetAverageHours(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportQueries.GetAverageHoursAsync(from, to, cancellationToken);
        return Ok(result);
    }
}