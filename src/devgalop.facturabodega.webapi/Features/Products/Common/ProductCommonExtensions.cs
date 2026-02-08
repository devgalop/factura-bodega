using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Products.AddProduct;
using devgalop.facturabodega.webapi.Features.Products.AddProductToStock;
using devgalop.facturabodega.webapi.Features.Products.EditProduct;
using devgalop.facturabodega.webapi.Features.Products.GetProducts;
using devgalop.facturabodega.webapi.Features.Products.GetStock;

namespace devgalop.facturabodega.webapi.Features.Products.Common
{
    public static class ProductCommonExtensions
    {
        public static WebApplicationBuilder RegisterProductFeatures(this WebApplicationBuilder builder)
        {
            builder.RegisterAddProductFeature()
                   .RegisterGetProductStockFeature()
                   .RegisterGetProductsFeature()
                   .RegisterEditProductFeature()
                   .RegisterAddProductStockFeature();
            
            return builder;
        }
    }
}