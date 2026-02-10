using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Products.Common;

namespace devgalop.facturabodega.webapi.Features.Invoices.Common
{
    public class InvoiceDetailEntity
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public float Subtotal => Quantity * Product.UnitPrice;

        public Guid ProductId { get; set; }
        public ProductEntity Product { get; set; }

        public Guid InvoiceId { get; set; }
        public InvoiceEntity Invoice { get; set; }

        public InvoiceDetailEntity()
        {
            Quantity = 0;
            Product = null!;
            Invoice = null!;
        }

        public InvoiceDetailEntity(int quantity, ProductEntity product, InvoiceEntity invoice)
        {
            Id = Guid.CreateVersion7();
            Quantity = quantity;
            Product = product;
            ProductId = product.Id;
            Invoice = invoice;
            InvoiceId = invoice.Id;
        }
    }
}