using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Domain;

namespace Application.UseCases.Reports;

public class ReportQueries
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public ReportQueries(IShiftRepository shiftRepository, IEmployeeRepository employeeRepository)
    {
        _shiftRepository = shiftRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<WorkingHoursDto> GetEmployeeHoursAsync(long employeeId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException($"Employee with ID {employeeId} not found.", "EMPLOYEE_NOT_FOUND");
        }

        var hours = await _shiftRepository.GetTotalHoursAsync(employeeId, fromDate, toDate, cancellationToken);
        return new WorkingHoursDto { Hours = hours };
    }

    public async Task<MostHoursReportDto> GetEmployeeWithMostHoursAsync(int month, int year, CancellationToken cancellationToken = default)
    {
        var result = await _shiftRepository.GetEmployeeWithMostHoursAsync(month, year, cancellationToken);
        
        return new MostHoursReportDto
        {
            Employee = result.Employee?.ToDto(),
            Hours = result.Hours
        };
    }

    public async Task<WorkingHoursDto> GetAverageHoursAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var hours = await _shiftRepository.GetAverageHoursAsync(fromDate, toDate, cancellationToken);
        return new WorkingHoursDto { Hours = hours };
    }
}