using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Packt.Ecommerce.Web.Contracts;
using Packt.Ecommerce.Web.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Packt.Ecommerce.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> logger;

        private readonly IECommerceService eCommerceService;

        public ProductsController(ILogger<ProductsController> logger, IECommerceService eCommerceService)
        {
            this.logger = logger;
            this.eCommerceService = eCommerceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString, string category)
        {
            this.ViewBag.SearchString = searchString;
            this.ViewBag.Category = category;
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = $"CONTAINS(e.Name,  '{searchString}')";
                if (!string.IsNullOrEmpty(category))
                {
                    searchString = $"{searchString} and CONTAINS(e.Category,  '{category}')";
                }
            }
            else if (!string.IsNullOrEmpty(category))
            {
                searchString = $"CONTAINS(e.Category,  '{category}')";
            }

            var products = await this.eCommerceService.GetProductsAsync(searchString).ConfigureAwait(false);
            return this.View(products);
        }


        [HttpGet]
        public async Task<IActionResult> Details(string productId, string productName)
        {
            var product = await this.eCommerceService.GetProductByIdAsync(productId, productName).ConfigureAwait(false);
            return this.View(product);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/Products/Error/{code:int}")]
        public IActionResult Error(int code)
        {
            if (code == 404)
            {
                return this.View("~/Views/Shared/NotFound.cshtml");
            }
            else
            {
                return this.View("~/Views/Shared/Error.cshtml", new ErrorViewModel { CorrelationId = Activity.Current?.RootId ?? this.HttpContext.TraceIdentifier });
            }
        }
    }
}
