using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Mango.Services.CouponAPI.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CouponService> _logger;
        private IMapper _mapper;

        public CouponService(ApplicationDbContext db, ILogger<CouponService> logger,IMapper mapper)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;

        }

        public async Task<string> CreateCoupon(CouponCreateDTO coupon)
        {
        
           var model=_mapper.Map<Coupon>(coupon);
            try
            {
                //check whether coupon exist or not--> for duplicate check
                if (await _db.Coupons.AsNoTracking().FirstOrDefaultAsync(u => u.CouponCode == coupon.CouponCode) != null)
                {
                    _logger.LogError($"Coupon code: {coupon.CouponCode} already exist. Please add a unique coupon code.");
                    return "Coupon exist";
                }
                model.CreatedDate = DateTime.Now;
                await _db.AddAsync(model);
                await _db.SaveChangesAsync();
                return model.CouponId.ToString();

            }
            catch (Exception ex)
            {
                return $"Exception out: {ex.Message}";
            }

        }

        public Task DeleteCoupon(int couponId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Coupon>> GetAllCoupons()
        {
            try 
            {
                _logger.LogInformation("Fetching Coupons");
                var coupons = await _db.Coupons.ToListAsync();
                if (coupons == null)
                {
                    return Enumerable.Empty<Coupon>();
                }
                return coupons;
            }
           catch (Exception ex) 
            {
                _logger.LogError($"Exception Out: {ex.Message}");
                return Enumerable.Empty<Coupon>();
            }

        }

        public async Task<Coupon> GetCoupons(int couponId)
        {
            try
            {
                _logger.LogInformation($"Fetching coupon with couponId: {couponId}");
                var coupon =await _db.Coupons.FirstOrDefaultAsync(u => u.CouponId == couponId);
                if (coupon == null)
                {
                    _logger.LogWarning($"No Coupon found with couponId: {couponId}");
                    return null;
                }
                return coupon;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception out: {ex.Message}");
                return null;
            }
        }

        public Task<Coupon> UpdateCoupon(CouponUpdateDTO coupon)
        {
            throw new NotImplementedException();
        }
    }
}
