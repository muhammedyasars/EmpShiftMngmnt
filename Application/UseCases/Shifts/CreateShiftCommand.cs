using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Domain;

namespace Application.UseCases.Shifts;

public class CreateShiftCommand
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateShiftCommand(
        IShiftRepository shiftRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _shiftRepository = shiftRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ShiftDto> ExecuteAsync(CreateShiftDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.StartTime >= dto.EndTime)
        {
            throw new ValidationException("Start time must be before end time.", "INVALID_TIME_RANGE");
        }

        var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException($"Employee with ID {dto.EmployeeId} not found.", "EMPLOYEE_NOT_FOUND");
        }

        if (!employee.IsActive)
        {
            throw new BusinessRuleException($"Cannot assign shift to inactive employee with ID {dto.EmployeeId}.", "EMPLOYEE_INACTIVE");
        }

        if (await _shiftRepository.HasOverlappingShiftsAsync(dto.EmployeeId, dto.StartTime, dto.EndTime, null, cancellationToken))
        {
            throw new BusinessRuleException("Shift overlaps with an existing shift.", "OVERLAP_ERROR");
        }

        var shift = new Shift
        {
            EmployeeId = dto.EmployeeId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdShift = await _shiftRepository.CreateAsync(shift, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return createdShift.ToDto();
    }
}