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
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository repository;


        public UsersController(IUserRepository repository)
        {
            this.repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync(string filterCriteria = null)
        {
            IEnumerable<User> users;
            if (string.IsNullOrEmpty(filterCriteria))
            {
                users = await this.repository.GetAsync(string.Empty).ConfigureAwait(false);
            }
            else
            {
                users = await this.repository.GetAsync(filterCriteria).ConfigureAwait(false);
            }

            if (users.Any())
            {
                return this.Ok(users);
            }
            else
            {
                return this.NoContent();
            }
        }

        
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(string id, [FromQuery][Required] string email)
        {
            User result = await this.repository.GetByIdAsync(id, email).ConfigureAwait(false);
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
        public async Task<IActionResult> CreateUserAsync([FromBody] User user)
        {
            if (user == null || user.Etag != null)
            {
                return this.BadRequest();
            }

            var result = await this.repository.AddAsync(user, user.Name).ConfigureAwait(false);
            return this.CreatedAtAction(nameof(this.GetUserById), new { id = result.Resource.Id, name = result.Resource.Name }, result.Resource);
        }

        
        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync([FromBody] User user)
        {
            if (user == null || user.Etag != null)
            {
                return this.BadRequest();
            }

            bool result = await this.repository.ModifyAsync(user, user.Etag, user.Name).ConfigureAwait(false);
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
        public async Task<IActionResult> DeleteUserAsync(string id, [FromQuery][Required] string email)
        {
            bool result = await this.repository.RemoveAsync(id, email).ConfigureAwait(false);
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
