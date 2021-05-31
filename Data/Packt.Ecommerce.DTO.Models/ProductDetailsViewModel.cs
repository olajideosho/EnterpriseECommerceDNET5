using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Packt.Ecommerce.DTO.Models
{
    public class ProductDetailsViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [Range(0,9999)]
        public int Price { get; set; }

        [Required]
        [Range(0, 999, ErrorMessage = "Large quantity, please reach out to support to process request.")]
        public int Quantity { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<string> ImageUrls { get; set; }

        public List<RatingViewModel> Rating { get; set; }

        public List<string> Format { get; set; }

        public List<string> Authors { get; set; }

        public List<int> Size { get; set; }

        public List<string> Color { get; set; }

        public string Etag { get; set; }
    }
}
