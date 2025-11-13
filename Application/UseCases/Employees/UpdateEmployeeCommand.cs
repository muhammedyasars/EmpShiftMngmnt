using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Domain;

namespace Application.UseCases.Employees;

public class UpdateEmployeeCommand
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommand(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EmployeeDto> ExecuteAsync(long id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        // Validate email format
        if (!IsValidEmail(dto.Email))
        {
            throw new ValidationException($"Email '{dto.Email}' is not in a valid format.", "INVALID_EMAIL_FORMAT");
        }

        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException($"Employee with ID {id} not found.", "EMPLOYEE_NOT_FOUND");
        }

        if (await _employeeRepository.EmailExistsAsync(dto.Email, id, cancellationToken))
        {
            throw new ValidationException($"Email '{dto.Email}' is already in use.", "EMAIL_EXISTS");
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.UpdatedAt = DateTime.UtcNow;

        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employee.ToDto();
    }

    public async Task<EmployeeDto> DeactivateAsync(long id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException($"Employee with ID {id} not found.", "EMPLOYEE_NOT_FOUND");
        }

        employee.IsActive = false;
        employee.UpdatedAt = DateTime.UtcNow;

        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employee.ToDto();
    }

    private static bool IsValidEmail(string email)
    {
        // Check if email contains required characters and has no spaces
        if (string.IsNullOrWhiteSpace(email))
            return false;

        if (email.Contains(' '))
            return false;

        if (!email.Contains('@'))
            return false;

        if (!email.Contains('.'))
            return false;

        // Basic email format validation
        var parts = email.Split('@');
        if (parts.Length != 2)
            return false;

        var localPart = parts[0];
        var domainPart = parts[1];

        if (string.IsNullOrWhiteSpace(localPart) || string.IsNullOrWhiteSpace(domainPart))
            return false;

        if (!domainPart.Contains('.'))
            return false;

        return true;
    }
}