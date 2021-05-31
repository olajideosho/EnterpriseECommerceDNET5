using System;
using System.ComponentModel.DataAnnotations;

namespace Packt.Ecommerce.DTO.Models
{
    public class AddressViewModel
    {
        [Required(ErrorMessage = "Address is required")]
        public string Address1 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
    }
}
