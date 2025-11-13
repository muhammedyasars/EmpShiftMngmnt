using Domain;

namespace Application.Common.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IEnumerable<Employee>> GetAllActiveAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, long? excludeId = null, CancellationToken cancellationToken = default);
}