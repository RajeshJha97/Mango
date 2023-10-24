using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon,CouponUpdateDTO>().ReverseMap();
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
        }
    }
}
