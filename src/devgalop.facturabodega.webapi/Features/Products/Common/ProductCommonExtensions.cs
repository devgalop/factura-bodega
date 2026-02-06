using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Products.AddProduct;
using devgalop.facturabodega.webapi.Features.Products.GetStock;

namespace devgalop.facturabodega.webapi.Features.Products.Common
{
    public static class ProductCommonExtensions
    {
        public static WebApplicationBuilder RegisterProductFeatures(this WebApplicationBuilder builder)
        {
            builder.RegisterAddProductFeature()
                   .RegisterGetProductStockFeature();
            
            return builder;
        }
    }
}