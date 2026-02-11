using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Customers.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;

namespace devgalop.facturabodega.webapi.Features.Invoices.Common
{
    public class InvoiceEntity
    {
        public Guid Id { get; set; }
        public DateTime DateOnUtc { get; set; }
        public float Total => Details.Sum(d => d.Subtotal);
        
        public Guid ClientId { get; set; }
        public CustomerEntity Client { get; set; }

        public Guid EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; }

        public ICollection<InvoiceDetailEntity> Details { get; set; }

        public InvoiceEntity()
        {
            DateOnUtc = DateTime.UtcNow;
            Client = null!;
            Employee = null!;
            Details = [];
        }

        public InvoiceEntity(CustomerEntity customer, EmployeeEntity employee)
        {
            Id = Guid.CreateVersion7();
            DateOnUtc = DateTime.UtcNow;
            Client = customer;
            ClientId = customer.Id;
            Employee = employee;
            EmployeeId = employee.Id;
            Details = [];
        }
    }
}