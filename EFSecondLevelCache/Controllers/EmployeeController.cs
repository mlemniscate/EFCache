using Bogus;
using EFSecondLevelCache.Infrustructure;
using EFSecondLevelCache.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Z.EntityFramework.Plus;

namespace EFSecondLevelCache.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext context;

        public EmployeeController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: EmployeeController
        [HttpGet]
        public ActionResult GetAll()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var employees =  context.Employees.ToList();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {employees.Count}" );
            return Ok($"Execution Time: {watch.ElapsedMilliseconds} ms => Count = {employees.Count}");
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

        // GET: EmployeeController/Create
        [HttpPost]
        public ActionResult CreateEmployees()
        {
            var userIds = 1;
            var fakeEmployees = new Faker<Employee>()
                .CustomInstantiator(f => new Employee())
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.FirstName, e.LastName));

            var employees = fakeEmployees.Generate(100_000).ToList();
            context.Employees.AddRange(employees);
            context.SaveChanges();
            QueryCacheManager.ExpireAll();
            return Ok();
        }

    }
}
