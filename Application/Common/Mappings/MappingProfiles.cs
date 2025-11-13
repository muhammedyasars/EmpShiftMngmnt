using Application.DTOs;
using Domain;

namespace Application.Common.Mappings;

public static class MappingProfiles
{
    public static EmployeeDto ToDto(this Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }

    public static ShiftDto ToDto(this Shift shift)
    {
        return new ShiftDto
        {
            Id = shift.Id,
            EmployeeId = shift.EmployeeId,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            IsDeleted = shift.IsDeleted,
            CreatedAt = shift.CreatedAt,
            UpdatedAt = shift.UpdatedAt
        };
    }

    public static IEnumerable<EmployeeDto> ToDtoList(this IEnumerable<Employee> employees)
    {
        return employees.Select(e => e.ToDto());
    }

    public static IEnumerable<ShiftDto> ToDtoList(this IEnumerable<Shift> shifts)
    {
        return shifts.Select(s => s.ToDto());
    }
}