using EFSecondLevelCache.Models;

namespace EFSecondLevelCache.Infrastructure.Employees;

public interface IEmployeeRepository
{
    IList<Employee> GetEmployees();
    IList<Employee> GetInMemoryCachedEmployees();
}