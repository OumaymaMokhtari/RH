using ChatAppBackend.Models;
using ChatAppBackend.ViewModels;

namespace ChatAppBackend.Mappers;

public static class EmployeeMapper
{
    public static EmployeeVM ToVM(Employee employee) => new()
    {
        Id = employee.Id,
        Name = employee.Name
    };
}
