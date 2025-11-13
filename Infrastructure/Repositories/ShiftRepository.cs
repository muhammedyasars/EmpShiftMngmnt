using Application.Common.Interfaces;
using Domain;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly ApplicationDbContext _context;

    public ShiftRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Shift> CreateAsync(Shift shift, CancellationToken cancellationToken = default)
    {
        _context.Shifts.Add(shift);
        return shift;
    }

    public async Task<Shift?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Shift>> GetAllAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 4,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Shifts
            .Include(s => s.Employee)
            .Where(s => !s.IsDeleted);

        if (fromDate.HasValue)
        {
            query = query.Where(s => s.EndTime >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(s => s.StartTime <= toDate.Value);
        }

        return await query
            .OrderBy(s => s.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Shift shift, CancellationToken cancellationToken = default)
    {
        _context.Shifts.Update(shift);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var shift = await _context.Shifts.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (shift != null)
        {
            shift.IsDeleted = true;
            _context.Shifts.Update(shift);
        }
    }

    public async Task<bool> HasOverlappingShiftsAsync(
        long employeeId,
        DateTime startTime,
        DateTime endTime,
        long? excludeShiftId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Shifts
            .Where(s => s.EmployeeId == employeeId && 
                        !s.IsDeleted &&
                        s.StartTime < endTime && 
                        startTime < s.EndTime);

        if (excludeShiftId.HasValue)
        {
            query = query.Where(s => s.Id != excludeShiftId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalHoursAsync(
        long employeeId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var totalMinutes = await _context.Shifts
            .Where(s => s.EmployeeId == employeeId &&
                        !s.IsDeleted &&
                        s.StartTime >= fromDate &&
                        s.EndTime <= toDate)
            .SumAsync(s => EF.Functions.DateDiffMinute(s.StartTime, s.EndTime), cancellationToken);

        return Math.Round((decimal)totalMinutes / 60, 2);
    }

    public async Task<(Employee? Employee, decimal Hours)> GetEmployeeWithMostHoursAsync(
        int month,
        int year,
        CancellationToken cancellationToken = default)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var employeeHours = await _context.Shifts
            .Where(s => !s.IsDeleted &&
                        s.StartTime >= startDate &&
                        s.EndTime <= endDate)
            .GroupBy(s => s.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                TotalMinutes = g.Sum(s => EF.Functions.DateDiffMinute(s.StartTime, s.EndTime))
            })
            .OrderByDescending(x => x.TotalMinutes)
            .FirstOrDefaultAsync(cancellationToken);

        if (employeeHours == null)
        {
            return (null, 0);
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeHours.EmployeeId, cancellationToken);

        var hours = Math.Round((decimal)employeeHours.TotalMinutes / 60, 2);
        return (employee, hours);
    }

    public async Task<decimal> GetAverageHoursAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var employeeHours = await _context.Shifts
            .Where(s => !s.IsDeleted &&
                        s.StartTime >= fromDate &&
                        s.EndTime <= toDate)
            .GroupBy(s => s.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                TotalMinutes = g.Sum(s => EF.Functions.DateDiffMinute(s.StartTime, s.EndTime))
            })
            .ToListAsync(cancellationToken);

        if (!employeeHours.Any())
        {
            return 0;
        }

        var totalHours = employeeHours.Sum(eh => (decimal)eh.TotalMinutes / 60);
        var averageHours = totalHours / employeeHours.Count;
        return Math.Round(averageHours, 2);
    }
}