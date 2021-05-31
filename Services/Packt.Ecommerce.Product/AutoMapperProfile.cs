using System;
using System.Linq;
using AutoMapper;

namespace Packt.Ecommerce.Product
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.MapEntity();
        }

        private void MapEntity()
        {
            this.CreateMap<Data.Models.Product, DTO.Models.ProductDetailsViewModel>();

            this.CreateMap<Data.Models.Rating, DTO.Models.RatingViewModel>();
            this.CreateMap<Data.Models.Product, DTO.Models.ProductListViewModel>()
                .ForMember(x => x.AverageRating, o => o.MapFrom(a => a.Rating != null && a.Rating.Count > 0 ? a.Rating.Average(y => y.Stars) : 0));
        }
    }
}
