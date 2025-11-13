using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Domain;

namespace Application.UseCases.Employees;

public class CreateEmployeeCommand
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommand(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EmployeeDto> ExecuteAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        // Validate email format
        if (!IsValidEmail(dto.Email))
        {
            throw new ValidationException($"Email '{dto.Email}' is not in a valid format.", "INVALID_EMAIL_FORMAT");
        }

        if (await _employeeRepository.EmailExistsAsync(dto.Email, null, cancellationToken))
        {
            throw new ValidationException($"Email '{dto.Email}' is already in use.", "EMAIL_EXISTS");
        }

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdEmployee = await _employeeRepository.CreateAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return createdEmployee.ToDto();
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