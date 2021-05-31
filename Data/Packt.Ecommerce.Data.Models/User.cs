using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Packt.Ecommerce.Data.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int PhoneNumber { get; set; }

        public List<Address> Address { get; set; }

        [JsonProperty("_etag")]
        public string Etag { get; set; }
    }
}
