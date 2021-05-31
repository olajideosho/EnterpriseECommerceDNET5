using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Packt.Ecommerce.DTO.Models;
using Packt.Ecommerce.Web.Contracts;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Packt.Ecommerce.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> logger;

        private readonly IECommerceService eCommerceService;

        //public CartController(ILogger<CartController> logger, IECommerceService eCommerceService, TelemetryClient telemetry)
        public CartController(ILogger<CartController> logger, IECommerceService eCommerceService)
        {
            this.logger = logger;
            this.eCommerceService = eCommerceService;
            //this.telemetry = telemetry;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProductListViewModel product)
        {
            if (product == null)
            {
                return this.BadRequest();
            }

            //this.telemetry.TrackEvent("Add Item To Cart");

            OrderDetailsViewModel newOrder = new OrderDetailsViewModel();
            if (this.ModelState.IsValid)
            {
                newOrder.UserId = "test";
                newOrder.Products = new List<ProductListViewModel>();
                product.Quantity = 1;
                newOrder.Products.Add(product);
                newOrder = await this.eCommerceService.CreateOrUpdateOrder(newOrder).ConfigureAwait(false);
            }

            return this.RedirectToAction("Index", new { orderId = newOrder.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Index(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return this.NotFound();
            }

            var newOrder = await this.eCommerceService.GetOrderByIdAsync(orderId).ConfigureAwait(false);
            if (newOrder != null && newOrder.OrderStatus == OrderStatus.Cart.ToString())
            {
                newOrder.PaymentMode = PaymentMode.Visa.ToString();
                return this.View(newOrder);
            }
            else
            {
                return this.NotFound();
            }
        }
    }
}
