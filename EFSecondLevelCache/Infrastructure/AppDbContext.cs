using EFSecondLevelCache.Models;
using Microsoft.EntityFrameworkCore;

namespace EFSecondLevelCache.Infrastructure;

public class AppDbContext : DbContext
{
    // private static readonly Func<AppDbContext, string, IEnumerable<Employee>> emplyeeContainedLetterCQ =
    //     EF.CompileQuery((AppDbContext context, string letter) =>
    //         context.Employees.Where(e => e.FirstName.Contains(letter) || e.LastName.Contains(letter)));

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Employee> Employees { get; set; }

    // public IEnumerable<Employee> GetEmployees(string letter)
    // {
    //     return emplyeeContainedLetterCQ(this, letter);
    // }
}