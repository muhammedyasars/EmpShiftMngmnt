using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.DTOs;
using Domain;

namespace Application.UseCases.Shifts;

public class GetShiftQuery
{
    private readonly IShiftRepository _shiftRepository;

    public GetShiftQuery(IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    public async Task<ShiftDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(id, cancellationToken);
        if (shift == null)
        {
            throw new NotFoundException($"Shift with ID {id} not found.", "SHIFT_NOT_FOUND");
        }

        return shift.ToDto();
    }

    public async Task<IEnumerable<ShiftDto>> GetAllAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var shifts = await _shiftRepository.GetAllAsync(fromDate, toDate, page, pageSize, cancellationToken);
        return shifts.ToDtoList();
    }
}