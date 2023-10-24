using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI.Service.IService
{
    public interface ICouponService
    {
        Task<IEnumerable<Coupon>> GetAllCoupons();
        Task<Coupon> GetCoupons(int couponId);
        Task<string> CreateCoupon(CouponCreateDTO coupon);
        Task<Coupon> UpdateCoupon(CouponUpdateDTO coupon);
        Task DeleteCoupon(int couponId);
    }
}
