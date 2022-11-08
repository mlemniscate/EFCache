using Bogus;
using EFSecondLevelCache.Models;
using Microsoft.EntityFrameworkCore;

namespace EFSecondLevelCache.Infrustructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Employee> Employees { get; set; }
}