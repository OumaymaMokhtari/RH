using ChatAppBackend.Models;
using ChatAppBackend.ViewModels;

namespace ChatAppBackend.Mappers;

public static class EmployeeMapper
{
    public static EmployeeVM ToVM(Employee employee) => new()
    {
        Id = employee.Id,
        EmployeeId = employee.EmployeeId,
        Name = employee.Name,
        Username = employee.Username,
        Password = employee.Password,
        Role = employee.Role,
        Cin = employee.Cin,
        DateNaissance = employee.DateNaissance,
        Email = employee.Email,
        Tele = employee.Tele,
        EmployeIdSql = employee.EmployeIdSql
    };
}
