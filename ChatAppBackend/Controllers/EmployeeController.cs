using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization; 
using ChatAppBackend.Data;
using ChatAppBackend.Mappers;
using ChatAppBackend.ViewModels;
using MongoDB.Driver;
using ChatAppBackend.Models;
// using System.Security.Claims; 

namespace ChatAppBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly MongoDbContext _context;

    public EmployeeController(MongoDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    // [Authorize]

    [HttpGet]
    public IActionResult GetEmployees()
    {
        // var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var employees = _context.Employees.Find(_ => true).ToList();

        return Ok(employees.Select(EmployeeMapper.ToVM));
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeVM employeeVm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var employee = new Employee
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Name = employeeVm.Name,
            Password = employeeVm.Password,
            Username = employeeVm.Username,
            Role = employeeVm.Role
        };

        await _context.Employees.InsertOneAsync(employee);
        return Ok(EmployeeMapper.ToVM(employee));
    }
}
