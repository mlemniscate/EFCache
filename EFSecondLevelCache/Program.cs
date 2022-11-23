using EFSecondLevelCache.Extensions;
using EFSecondLevelCache.Infrastructure;
using EFSecondLevelCache.Infrastructure.Employees;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMyCors();

// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
// EFSecondLevelCache service adding
// builder.Services.AddEFSecondLevelCache(options =>
// {
//     options.UseMemoryCacheProvider().DisableLogging(false).UseCacheKeyPrefix("EF_");
//     options.CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30));
// });

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// builder.Services.AddConfiguredMsSqlDbContext(builder.Configuration.GetConnectionString("Default"));
// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddResponseCaching();

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

app.UseCors("CorsPolicy");

app.UseResponseCaching();

app.MapControllers();

app.Run();
