using Domain;

namespace Application.Common.Interfaces;

public interface IShiftRepository
{
    Task<Shift> CreateAsync(Shift shift, CancellationToken cancellationToken = default);

    Task<Shift?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Shift>> GetAllAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Shift shift, CancellationToken cancellationToken = default);

    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingShiftsAsync(
        long employeeId,
        DateTime startTime,
        DateTime endTime,
        long? excludeShiftId = null,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalHoursAsync(
        long employeeId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    Task<(Employee? Employee, decimal Hours)> GetEmployeeWithMostHoursAsync(
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task<decimal> GetAverageHoursAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
}