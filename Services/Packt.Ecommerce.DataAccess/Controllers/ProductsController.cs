using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Packt.Ecommerce.Data.Models;
using Packt.Ecommerce.DataStore.Contracts;

namespace Packt.Ecommerce.DataAccess.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository repository;

        public ProductsController(IProductRepository repository)
        {
            this.repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProductAsync(string filterCriteria = null)
        {
            IEnumerable<Product> products;
            if (string.IsNullOrEmpty(filterCriteria))
            {
                products = await this.repository.GetAsync(string.Empty).ConfigureAwait(false);
            }
            else
            {
                products = await this.repository.GetAsync(filterCriteria).ConfigureAwait(false);
            }

            if (products.Any())
            {
                return this.Ok(products);
            }
            else
            {
                return this.NoContent();
            }
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProductById(string id, [FromQuery][Required] string name)
        {
            Product result = await this.repository.GetByIdAsync(id, name).ConfigureAwait(false);
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
        public async Task<IActionResult> CreateProductAsync([FromBody] Product product)
        {
            if (product == null || product.Etag != null)
            {
                return this.BadRequest();
            }

            var result = await this.repository.AddAsync(product, product.Name).ConfigureAwait(false);
            return this.CreatedAtAction(nameof(this.GetProductById), new { id = result.Resource.Id, name = result.Resource.Name }, result.Resource);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync([FromBody] Product product)
        {
            if (product == null || product.Etag == null || product.Id == null)
            {
                return this.BadRequest();
            }

            bool result = await this.repository.ModifyAsync(product, product.Etag, product.Name).ConfigureAwait(false);
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
        public async Task<IActionResult> DeleteProductAsync(string id, [FromQuery][Required] string name)
        {
            bool result = await this.repository.RemoveAsync(id, name).ConfigureAwait(false);
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
