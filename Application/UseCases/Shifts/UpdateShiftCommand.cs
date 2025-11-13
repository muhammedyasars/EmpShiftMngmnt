using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Domain;

namespace Application.UseCases.Shifts;

public class UpdateShiftCommand
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShiftCommand(
        IShiftRepository shiftRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _shiftRepository = shiftRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ShiftDto> ExecuteAsync(long id, UpdateShiftDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.StartTime >= dto.EndTime)
        {
            throw new ValidationException("Start time must be before end time.", "INVALID_TIME_RANGE");
        }

        var shift = await _shiftRepository.GetByIdAsync(id, cancellationToken);
        if (shift == null)
        {
            throw new NotFoundException($"Shift with ID {id} not found.", "SHIFT_NOT_FOUND");
        }

        if (await _shiftRepository.HasOverlappingShiftsAsync(shift.EmployeeId, dto.StartTime, dto.EndTime, id, cancellationToken))
        {
            throw new BusinessRuleException("Shift overlaps with an existing shift.", "OVERLAP_ERROR");
        }

        shift.StartTime = dto.StartTime;
        shift.EndTime = dto.EndTime;
        shift.UpdatedAt = DateTime.UtcNow;

        await _shiftRepository.UpdateAsync(shift, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return shift.ToDto();
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(id, cancellationToken);
        if (shift == null)
        {
            throw new NotFoundException($"Shift with ID {id} not found.", "SHIFT_NOT_FOUND");
        }

        await _shiftRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}