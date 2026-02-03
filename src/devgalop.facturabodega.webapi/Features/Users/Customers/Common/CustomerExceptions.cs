using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Customers.Common
{
    public class CustomerAlreadyExistsException : Exception
    {
        public string Email {get;}
        public string Document {get;}

        public CustomerAlreadyExistsException(string email, string document) 
                    : base($"Un cliente con el email '{email}' o documento '{document}' ya existe en el sistema.")
        {
            Email = email;
            Document = document;
        }
    }

    public class CustomerNotFoundException : Exception
    {
        public string Id {get;}

        public CustomerNotFoundException(string id) 
                    : base($"El cliente con Id '{id}' no se encuentra registrado en el sistema.")
        {
            Id = id;
        }
    }
}