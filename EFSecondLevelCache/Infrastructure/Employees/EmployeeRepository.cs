using EFSecondLevelCache.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EFSecondLevelCache.Infrastructure.Employees;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext context;
    private readonly IMemoryCache memoryCache;

    public EmployeeRepository(AppDbContext context,
        IMemoryCache memoryCache)
    {
        this.context = context;
        this.memoryCache = memoryCache;
    }

    public IList<Employee> GetEmployees()
    {
        return context.Employees.ToList();
    }

    public IList<Employee> GetInMemoryCachedEmployees()
    {
        List<Employee> employees;

        employees = memoryCache.Get<List<Employee>>("employees");

        if (employees is null)
        {
            employees = context.Employees.ToList();

            memoryCache.Set("employees", employees, TimeSpan.FromDays(1));
        }
        return employees;
    }
}