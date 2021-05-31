using System;
using AutoMapper;

namespace Packt.Ecommerce.Invoice
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.MapEntity();
        }

        /// <summary>
        /// Maps entities.
        /// </summary>
        private void MapEntity()
        {
            this.CreateMap<Data.Models.Invoice, DTO.Models.InvoiceDetailsViewModel>();
            this.CreateMap<Data.Models.Product, DTO.Models.ProductListViewModel>();
            this.CreateMap<Data.Models.Address, DTO.Models.AddressViewModel>();
            this.CreateMap<Data.Models.SoldBy, DTO.Models.SoldByViewModel>();
        }
    }
}
