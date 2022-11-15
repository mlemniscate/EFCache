using Bogus;
using EFSecondLevelCache.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using EFCoreSecondLevelCacheInterceptor;
using EFSecondLevelCache.Infrastructure;
using EFSecondLevelCache.Infrastructure.Employees;
using Z.EntityFramework.Plus;
using Microsoft.EntityFrameworkCore;

namespace EFSecondLevelCache.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext context;
        private readonly IEmployeeRepository repository;

        public EmployeeController(AppDbContext context,
            IEmployeeRepository repository)
        {
            this.context = context;
            this.repository = repository;
        }

        // GET: EmployeeController
        [HttpGet]
        public ActionResult GetAll()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees = repository.GetEmployees();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {employees.Count}" );
            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {employees.Count}");
        }

        [HttpGet]
        public ActionResult GetAllCount()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var count = context.Employees.Count();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {count}");
            watch.Reset();
            watch.Start();
            var count2 = context.Employees.DeferredCount().FromCache();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {count}");
            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {count}");
        }

        [HttpGet]
        public ActionResult GetAllEmails()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var count = context.Employees.Select(e => e.Email).ToList();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {count}");
            watch.Reset();
            watch.Start();
            var count2 = context.Employees.Select(e => new { e.Email }).FromCache().ToList();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {count}");
            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {count}");
        }

        [HttpGet]
        public ActionResult GetAllByInterceptor()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees = context.Employees.Where(x => x.Id > 0).OrderBy(x => x.Id)
                .FromCache().ToList();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedTicks} t => Count = {employees.Count}");

            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {employees.Count}");
        }

        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public ActionResult GetAllByApiCaching()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees = context.Employees.Where(x => x.Id > 0).OrderBy(x => x.Id)
                .FromCache().ToList();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedTicks} t => Count = {employees.Count}");

            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {employees.Count}");
        }

        [HttpGet]
        public ActionResult GetById(int id)
        {
            
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employe = context.Employees.Find(id);
            watch.Stop();
            Console.WriteLine($"Execution Time 1: {watch.ElapsedTicks} ticks");
            employe.FirstName = "Milad";
            context.SaveChanges();
            watch.Reset();
            watch.Start();
            context.Employees.Find(id);
            watch.Stop();
            Console.WriteLine($"Execution Time 2: {watch.ElapsedTicks} ticks");
            Console.WriteLine(context.GetHashCode());
            return Ok($"Execution Time: {watch.ElapsedTicks} ticks");
        }
        
        [HttpGet]
        public ActionResult GetAllEmployeesByZFramework()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees = context.Employees.FromCache(TableName.Employees.Value).ToList();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }

        [HttpGet]
        public ActionResult GetEmployeeByIdZFramework(int id)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees = context.Employees.DeferredFirst().FromCache(TableName.Employees.Value);
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }

        [HttpGet]
        public ActionResult GetEmployeesByInMemoryCache(string letter)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees = repository.GetInMemoryCachedEmployees();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }

        // GET: EmployeeController/Create
        [HttpPost]
        public ActionResult CreateEmployees()
        {
            var userIds = 1;
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var fakeEmployees = new Faker<Employee>()
                .CustomInstantiator(f => new Employee())
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.FirstName, e.LastName));
            watch.Stop();
            var employees = fakeEmployees.Generate(100_000).ToList();
            Console.WriteLine($"Execution Time For Faker: {watch.ElapsedMilliseconds} ms");

            watch.Reset();
            watch.Start();
            context.Employees.AddRange(employees);
            context.SaveChanges();
            watch.Stop();
            Console.WriteLine($"Execution Time For Adding to DB: {watch.ElapsedMilliseconds} ms");

            QueryCacheManager.ExpireTag(TableName.Employees.Value);
            return Ok();
        }

    }
}
