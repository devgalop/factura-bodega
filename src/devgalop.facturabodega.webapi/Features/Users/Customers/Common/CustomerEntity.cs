using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Customers.Common
{

    /// <summary>
    /// Entidad para representar a un cliente.
    /// </summary>
    public class CustomerEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }

        public CustomerEntity()
        {
            Name = string.Empty;
            Email = string.Empty;
            Document = string.Empty;
        }

        public CustomerEntity(string name, string email, string document)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Email = email;
            Document = document;
        }
    }
}