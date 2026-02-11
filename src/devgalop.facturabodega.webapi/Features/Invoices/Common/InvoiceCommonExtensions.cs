using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Invoices.CreateInvoice;

namespace devgalop.facturabodega.webapi.Features.Invoices.Common
{
    public static class InvoiceCommonExtensions
    {
        public static WebApplicationBuilder RegisterInvoiceFeature(this WebApplicationBuilder builder)
        {
            builder.RegisterCreateInvoiceFeature();
            return builder;
        }
    }
}