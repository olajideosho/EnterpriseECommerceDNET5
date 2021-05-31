using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Packt.Ecommerce.DTO.Models;
using Packt.Ecommerce.Web.Contracts;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Packt.Ecommerce.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> logger;

        private readonly IECommerceService eCommerceService;

        //public OrdersController(ILogger<OrdersController> logger, IECommerceService eCommerceService, TelemetryClient telemetry)
        public OrdersController(ILogger<OrdersController> logger, IECommerceService eCommerceService)
        {
            this.logger = logger;
            this.eCommerceService = eCommerceService;
            //this.telemetry = telemetry;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetailsViewModel order)
        {
            //this.telemetry.TrackEvent("Create Order");
            if (this.ModelState.IsValid)
            {
                InvoiceDetailsViewModel invoice;
                invoice = await this.eCommerceService.SubmitOrder(order).ConfigureAwait(false);
                return this.RedirectToAction("Index", new { invoiceId = invoice.Id });
            }

            return this.RedirectToAction("Index", "Cart", new { orderId = order.Id });
        }

        public async Task<IActionResult> Index(string invoiceId)
        {
            var invoice = await this.eCommerceService.GetInvoiceByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice != null)
            {
                return this.View(invoice);
            }
            else
            {
                return this.NotFound();
            }
        }
    }
}
