using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Packt.Ecommerce.DTO.Models
{
    public class OrderDetailsViewModel
    {
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public List<ProductListViewModel> Products { get; set; }

        public string OrderStatus { get; set; }

        public string OrderPlacedDate { get; set; }

        public AddressViewModel ShippingAddress { get; set; }

        public int TrackingId { get; set; }

        public string DeliveryDate { get; set; }

        public string Etag { get; set; }

        public double OrderTotal { get; set; }

        public string PaymentMode { get; set; }
    }
}
