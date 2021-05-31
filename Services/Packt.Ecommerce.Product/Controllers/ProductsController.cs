using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Packt.Ecommerce.DTO.Models;
using Packt.Ecommerce.Product.Contracts;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Packt.Ecommerce.Product.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }


        [HttpGet]
        public async Task<IActionResult> GetProductsAsync([FromQuery] string filterCriteria = null)
        {
            var products = await this.productService.GetProductsAsync(filterCriteria).ConfigureAwait(false);
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
            var product = await this.productService.GetProductByIdAsync(id, name).ConfigureAwait(false);
            if (product != null)
            {
                return this.Ok(product);
            }
            else
            {
                return this.NoContent();
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddProductAsync(ProductDetailsViewModel product)
        {
            if (product == null || product.Etag != null)
            {
                return this.BadRequest();
            }

            var result = await this.productService.AddProductAsync(product).ConfigureAwait(false);
            return this.CreatedAtAction(nameof(this.GetProductById), new { id = result.Id, name = result.Name }, result);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync(ProductDetailsViewModel product)
        {
            if (product == null || product.Etag == null || product.Id == null)
            {
                return this.BadRequest();
            }

            var result = await this.productService.UpdateProductAsync(product).ConfigureAwait(false);
            if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
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
            var result = await this.productService.DeleteProductAsync(id, name).ConfigureAwait(false);
            if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
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
