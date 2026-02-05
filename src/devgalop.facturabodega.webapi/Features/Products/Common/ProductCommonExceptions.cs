using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Products.Common
{
    public class ExistingProductException : Exception
    {
        public string Name {get;}
        public ExistingProductException(string name)
            : base($"Un producto con el mismo nombre '{name}' ya está registrado.")
        {
            Name = name;
        }
    }
}