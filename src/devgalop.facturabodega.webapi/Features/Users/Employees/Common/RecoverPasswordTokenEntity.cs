using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    public class RecoverPasswordTokenEntity
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOnUtc { get; set; }
        public bool IsUsed { get; set; }

        public Guid EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; }

        public RecoverPasswordTokenEntity()
        {
            Token = string.Empty;
            Employee = null!;
            IsUsed = false;
        }

        public RecoverPasswordTokenEntity(string token, DateTime expiresOnUtc, EmployeeEntity employee)
        {
            Id = Guid.CreateVersion7();
            Token = token;
            ExpiresOnUtc = expiresOnUtc;
            Employee = employee;
            EmployeeId = employee.Id;
            IsUsed = false;
        }
    }
}