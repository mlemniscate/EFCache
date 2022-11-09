using EFSecondLevelCache.Infrastructure;
using EFSecondLevelCache.Infrastructure.Employees;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

var app = builder.Build();

using (var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>())
{
    context.Database.EnsureCreated();
    context.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
