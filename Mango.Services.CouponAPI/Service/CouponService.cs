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

        public async Task<string> DeleteCoupon(int couponId)
        {
            try 
            {
              var coupon= await _db.Coupons.FirstOrDefaultAsync(u=>u.CouponId == couponId);
                if (coupon != null) 
                {
                   _db.Remove(coupon);
                    await _db.SaveChangesAsync();
                    return "";
                }
                _logger.LogWarning($"No coupon is available with coupon Id:{couponId}");

                return $"No coupon is available with coupon Id:{couponId}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return $"Exception out: {ex.Message}";
            }
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

        public async Task<Coupon> UpdateCoupon(CouponUpdateDTO coupon)
        {
            try 
            {
                var model=_mapper.Map<Coupon>(coupon);
                Coupon data = await _db.Coupons.AsNoTracking().FirstAsync(u => u.CouponId == coupon.CouponId);
                model.CreatedDate=data.CreatedDate;
                model.LastUpdatedDate=DateTime.Now;
                _db.Update(model);
                await _db.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception out: {ex.Message}");
                return null;
            }
        }
    }
}
