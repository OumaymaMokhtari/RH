using Microsoft.AspNetCore.Mvc;
using ChatAppBackend.Data;
using ChatAppBackend.Mappers;
using ChatAppBackend.ViewModels;
using MongoDB.Driver;
using ChatAppBackend.Models;

namespace ChatAppBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly MongoDbContext _context;

    public EmployeeController(MongoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // 🔹 GET /api/employee
    [HttpGet]
    public IActionResult GetEmployees()
    {
        var employees = _context.Employees.Find(_ => true).ToList();
        return Ok(employees.Select(EmployeeMapper.ToVM));
    }

    // 🔹 GET /api/employee/contacts/{sqlId}
    [HttpGet("contacts/{sqlId}")]
    public IActionResult GetContacts(string sqlId)
    {
        // 1. Trouver l'employé MongoDB lié à l'utilisateur SQL
        var currentUser = _context.Employees.Find(e => e.EmployeIdSql == sqlId).FirstOrDefault();
        if (currentUser == null)
            return NotFound("Utilisateur non trouvé dans MongoDB");

        // 2. Retourner tous les autres employés
        var contacts = _context.Employees.Find(e => e.EmployeIdSql != sqlId).ToList();
        return Ok(contacts.Select(EmployeeMapper.ToVM));
    }

    // 🔹 POST /api/employee
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
            EmployeeId = employeeVm.EmployeeId,
            Name = employeeVm.Name,
            Username = employeeVm.Username,
            Password = employeeVm.Password,
            Role = employeeVm.Role,
            Cin = employeeVm.Cin,
            DateNaissance = employeeVm.DateNaissance,
            Email = employeeVm.Email,
            EmployeIdSql = employeeVm.EmployeIdSql,
            Tele = employeeVm.Tele
        };

        await _context.Employees.InsertOneAsync(employee);
        return Ok(EmployeeMapper.ToVM(employee));
    }
}
