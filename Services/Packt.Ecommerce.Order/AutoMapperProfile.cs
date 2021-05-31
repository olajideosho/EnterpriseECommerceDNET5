using System;
using AutoMapper;

namespace Packt.Ecommerce.Order
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.MapEntity();
        }

        private void MapEntity()
        {
            this.CreateMap<Data.Models.Order, DTO.Models.OrderDetailsViewModel>();
            this.CreateMap<Data.Models.Product, DTO.Models.ProductListViewModel>();
            this.CreateMap<Data.Models.Address, DTO.Models.AddressViewModel>();
        }
    }
}
