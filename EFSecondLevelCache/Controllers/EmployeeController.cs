using Bogus;
using EFSecondLevelCache.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using EFSecondLevelCache.Infrastructure;
using EFSecondLevelCache.Infrastructure.Employees;
using Z.EntityFramework.Plus;

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
        public ActionResult GetAllByInMemoryCache()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees = repository.GetInMemoryCachedEmployees();
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
        public ActionResult GetAllFromCache()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees = context.Employees.FromCache().ToList();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {employees.Count}");
            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {employees.Count}");
        }

        // [HttpGet]
        // public ActionResult GetEmployeesByCompiledQuery(string letter)
        // {
        //     var watch = new System.Diagnostics.Stopwatch();
        //     watch.Start();
        //     var employees = context.GetEmployees(letter);
        //     watch.Stop();
        //     Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        //     return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms");
        // }

        [HttpGet]
        public ActionResult GetEmployeesWithoutCompiledQuery(string letter)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees = context.Employees.Where(e => e.FirstName.Contains(letter) || e.LastName.Contains(letter));
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
            QueryCacheManager.ExpireAll();
            return Ok();
        }

    }
}
