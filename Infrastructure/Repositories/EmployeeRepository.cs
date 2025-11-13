using Application.Common.Interfaces;
using Domain;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Add(employee);
        return employee;
    }

    public async Task<Employee?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetAllActiveAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(e => e.IsActive)
            .OrderBy(e => e.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(employee);
    }

    public async Task<bool> EmailExistsAsync(string email, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        return excludeId.HasValue
            ? await _context.Employees.AnyAsync(e => e.Email == email && e.Id != excludeId.Value, cancellationToken)
            : await _context.Employees.AnyAsync(e => e.Email == email, cancellationToken);
    }
}