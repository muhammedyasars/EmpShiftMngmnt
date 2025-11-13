using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Domain;

namespace Application.UseCases.Employees;

public class GetEmployeeQuery
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeQuery(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException($"Employee with ID {id} not found.", "EMPLOYEE_NOT_FOUND");
        }

        return employee.ToDto();
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllActiveAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllActiveAsync(page, pageSize, cancellationToken);
        return employees.ToDtoList();
    }
}