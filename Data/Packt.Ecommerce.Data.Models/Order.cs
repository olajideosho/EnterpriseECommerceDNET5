using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Packt.Ecommerce.Data.Models
{
    public class Order
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string UserId { get; set; }

        public List<Product> Products { get; set; }

        public string OrderStatus { get; set; }

        public string OrderPlacedDate { get; set; }

        public Address ShippingAddress { get; set; }

        public int TrackingId { get; set; }

        public string DeliveryDate { get; set; }

        public double OrderTotal { get; set; }

        public string PaymentMode { get; set; }

        [JsonProperty("_etag")]
        public string Etag { get; set; }
    }
}
