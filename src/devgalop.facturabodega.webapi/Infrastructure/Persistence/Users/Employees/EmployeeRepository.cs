using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Infrastructure.Persistence.Users.Employees
{
    public sealed class EmployeeRepository(AppDatabaseContext dbContext) :
        ICreateEmployeeRepository,
        IGetEmployeeRepository,
        IEditEmployeeRepository,
        IRemoveEmployeeRepository
    {
        public async Task CreateEmployee(EmployeeEntity employee)
        {
            dbContext.Employees.Add(employee);
            await dbContext.SaveChangesAsync();
        }

        public async Task EditEmployee(EmployeeEntity employee)
        {
            dbContext.Employees.Update(employee);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<EmployeeEntity>> GetActiveEmployees()
        {
            var activeEmployees = await dbContext.Employees
                .Include(e => e.Role)
                .Where(e => e.Status == EmployeeStatus.ACTIVE)
                .ToListAsync();
            return activeEmployees;
        }

        public async Task<List<EmployeeEntity>> GetAllEmployees()
        {
            var employees = await dbContext.Employees
                .Include(e => e.Role)
                .ToListAsync();
            return employees;
        }

        public async Task<EmployeeEntity?> GetEmployeeByEmail(string email)
        {
            var employee = await dbContext.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Email == email);
            return employee;
        }

        public async Task<EmployeeEntity?> GetEmployeeById(string id)
        {
            var employee = await dbContext.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Id.ToString() == id);
            return employee;
        }

        public async Task<List<EmployeeEntity>> GetEmployeesByStatus(EmployeeStatus status)
        {
            var employees = await dbContext.Employees
                .Include(e => e.Role)
                .Where(e => e.Status == status)
                .ToListAsync();
            return employees;
        }

        public async Task<List<EmployeeEntity>> GetInactiveEmployees()
        {
            var inactiveEmployees = await dbContext.Employees
                .Include(e => e.Role)
                .Where(e => e.Status == EmployeeStatus.INACTIVE)
                .ToListAsync();
            return inactiveEmployees;
        }

        public async Task RemoveEmployee(EmployeeEntity employee)
        {
            employee.Status = EmployeeStatus.INACTIVE;
            dbContext.Employees.Update(employee);
            await dbContext.SaveChangesAsync();
        }
    }
}