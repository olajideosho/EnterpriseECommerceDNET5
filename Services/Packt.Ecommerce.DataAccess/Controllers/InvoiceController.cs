using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Packt.Ecommerce.Data.Models;
using Packt.Ecommerce.DataStore.Contracts;

namespace Packt.Ecommerce.DataAccess.Controllers
{
    [Route("api/invoice")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository repository;

        public InvoiceController(IInvoiceRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoiceAsync(string filterCriteria = null)
        {
            IEnumerable<Invoice> invoice;
            if (string.IsNullOrEmpty(filterCriteria))
            {
                invoice = await this.repository.GetAsync(string.Empty).ConfigureAwait(false);
            }
            else
            {
                invoice = await this.repository.GetAsync(filterCriteria).ConfigureAwait(false);
            }

            if (invoice.Any())
            {
                return this.Ok(invoice);
            }
            else
            {
                return this.NoContent();
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetInvoiceById(string id)
        {
            Invoice result = await this.repository.GetByIdAsync(id, id).ConfigureAwait(false);
            if (result != null)
            {
                return this.Ok(result);
            }
            else
            {
                return this.NoContent();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoiceAsync([FromBody] Invoice invoice)
        {
            if (invoice == null || invoice.Etag != null)
            {
                return this.BadRequest();
            }

            var result = await this.repository.AddAsync(invoice, invoice.Id).ConfigureAwait(false);
            return this.CreatedAtAction(nameof(this.GetInvoiceById), new { id = result.Resource.Id }, result.Resource);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync([FromBody] Invoice invoice)
        {
            if (invoice == null || invoice.Etag != null)
            {
                return this.BadRequest();
            }

            bool result = await this.repository.ModifyAsync(invoice, invoice.Etag, invoice.Id).ConfigureAwait(false);
            if (result)
            {
                return this.Accepted();
            }
            else
            {
                return this.NoContent();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProductAsync(string id)
        {
            bool result = await this.repository.RemoveAsync(id, id).ConfigureAwait(false);
            if (result)
            {
                return this.Accepted();
            }
            else
            {
                return this.NoContent();
            }
        }
    }
}
