using System;
using System.Collections.Generic;

namespace Packt.Ecommerce.DTO.Models
{
    public class InvoiceDetailsViewModel
    {
        public string Id { get; set; }

        public string OrderId { get; set; }

        public string PaymentMode { get; set; }

        public AddressViewModel ShippingAddress { get; set; }

        public SoldByViewModel SoldBy { get; set; }

        public List<ProductListViewModel> Products { get; set; }

        public string Etag { get; set; }
    }
}
