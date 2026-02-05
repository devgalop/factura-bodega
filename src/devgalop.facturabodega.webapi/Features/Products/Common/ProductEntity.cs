using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Products.Common
{
    public sealed class ProductEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float UnitPrice { get; set; }

        public ProductEntity()
        {
            Name = string.Empty;
            Description = string.Empty;
            UnitPrice = 0;
        }

        public ProductEntity(string name, string description, float price)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Description = description;
            UnitPrice = price;
        }
    }

    public sealed class ProductStockEntity
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }

        public Guid ProductId { get; set; }
        public ProductEntity? Product { get; set; }
    }
}