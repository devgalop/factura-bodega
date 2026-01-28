using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    public sealed class EmployeeRefreshTokenEntity
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOnUtc { get; set; }

        public Guid EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; }

        private EmployeeRefreshTokenEntity()
        {
            Token = string.Empty;
            Employee = null!;
        }

        public EmployeeRefreshTokenEntity(string token, DateTime expiresOnUtc, EmployeeEntity employee)
        {
            Id = Guid.NewGuid();
            Token = token;
            ExpiresOnUtc = expiresOnUtc;
            Employee = employee;
            EmployeeId = employee.Id;
        }
    }
}