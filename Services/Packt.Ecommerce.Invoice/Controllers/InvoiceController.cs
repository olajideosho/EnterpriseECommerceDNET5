using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Packt.Ecommerce.DTO.Models;
using Packt.Ecommerce.Invoice.Contracts;

namespace Packt.Ecommerce.Invoice.Controllers
{
    [Route("api/Invoice")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            this.invoiceService = invoiceService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetInvoiceById(string id)
        {
            var invoice = await this.invoiceService.GetInvoiceByIdAsync(id).ConfigureAwait(false);
            if (invoice != null)
            {
                return this.Ok(invoice);
            }
            else
            {
                return this.NoContent();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddInvoiceAsync(InvoiceDetailsViewModel invoice)
        {
            if (invoice == null || invoice.Etag != null)
            {
                return this.BadRequest();
            }

            var result = await this.invoiceService.AddInvoiceAsync(invoice).ConfigureAwait(false);
            return this.CreatedAtAction(nameof(this.GetInvoiceById), new { id = result.Id }, result);
        }
    }
}
