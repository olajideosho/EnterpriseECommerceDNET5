using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Packt.Ecommerce.DTO.Models
{
    public class ProductListViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public Uri ImageUri { get; set; }

        public List<Uri> ImageUrls { get; set; }

        public int Quantity { get; set; }

        public double AverageRating { get; set; }
    }
}
