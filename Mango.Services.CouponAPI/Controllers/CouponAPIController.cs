using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Mango.Services.CouponAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static Azure.Core.HttpHeader;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        #region private variables
        private readonly ApplicationDbContext _db;
        private ResponseDTO _resp;
        private readonly IMapper _mapper;
        private ILogger<CouponAPIController> _logger;
        private readonly ICouponService _couponService;
        #endregion

        #region Constructor
        public CouponAPIController(ApplicationDbContext db,IMapper mapper,ILogger<CouponAPIController>logger, ICouponService couponService)
        {
            _db = db;
            _resp = new();
            _mapper = mapper;
            _logger = logger;
            _couponService= couponService;
        }
        #endregion

        #region GetAllCoupon
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseDTO>> GetAllCoupon()
        {
            try
            {
               var coupons=await _couponService.GetAllCoupons();
                if (coupons == null)
                {
                    _resp.Message = "No record found";
                    return BadRequest(_resp);
                }
                _resp.IsSuccess = true;
                _resp.Result = coupons;
                _logger.LogInformation("Data fetched");
                return Ok(_resp);
            }
            catch (Exception ex)
            {
                _resp.Message =ex.Message;
                return BadRequest(_resp);
            }
            
        }

        #endregion

        #region GetCouponByID
        //[HttpGet("{id:int}",Name = "GetCouponByID")]
        [HttpGet]
        [Route("{id:int}",Name = "GetCouponByID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCouponByID(int id)
        {
            try 
            {
                if (id == 0)
                {
                    _resp.Message = "No Data found";
                    return NotFound(_resp);

                }
                var data = await _couponService.GetCoupons(id);
                if (data == null)
                {
                    _resp.Message = $"No coupon found with id: {id}";
                    return NotFound(_resp);
                }
                _resp.Result = data;
                _resp.IsSuccess = true;                
                return Ok(_resp);
            }
            catch (Exception ex)
            {
                _resp.Message = ex.Message;
                return BadRequest(_resp);
            }

        }

        #endregion

        #region CreateCoupon
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ResponseDTO>> CreateCoupon([FromBody] CouponCreateDTO coupon)
        {
            try 
            {
                if (coupon is null)
                {
                    _resp.Message = "Data is null";
                    return BadRequest(_resp);
                }
               var responseFromCouponCreation=await _couponService.CreateCoupon(coupon);
                if (int.TryParse(responseFromCouponCreation, out int cId)) //here we are getting response id as a respon from db we are trying to convert into int if we get any error we will get string as a reponse which will not be passed to tryparse
                {
                    _resp.IsSuccess= true;
                    _logger.LogInformation($"Coupon created at:https://localhost:7001/api/CouponAPI/GetCouponByID/{cId} ");
                    return CreatedAtRoute("GetCouponByID", new { id =  cId}, _resp);
                }
                _resp.Message = responseFromCouponCreation;
                return BadRequest(_resp);
            }
            catch (Exception ex) 
            {
                _resp.Message = ex.Message;
                return BadRequest(_resp);
            }
            
        }
        #endregion

        #region UpdateCoupon
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ResponseDTO>> UpdateCoupon([FromBody] CouponUpdateDTO coupon)
        {
            try
            {
                if (coupon is null)
                {
                    _resp.Message = "Data is null";
                    return BadRequest(_resp);
                }
                Coupon data = await _db.Coupons.AsNoTracking().FirstAsync(u => u.CouponId == coupon.CouponId);
                //checking for duplicate coupon
                if (data is null)
                {
                    _resp.Message = $"Coupon code: {coupon.CouponId} doesn't exist. Please provide a valid data.";
                    return BadRequest(_resp);
                }
                

                //mapping
                Coupon model = _mapper.Map<Coupon>(coupon);
                model.CreatedDate = data.CreatedDate;
                model.LastUpdatedDate=DateTime.Now;
                 _db.Update(model);
                await _db.SaveChangesAsync();
                _resp.IsSuccess = true;
                _resp.Message = $"Coupon: {coupon.CouponCode} updated successfully";
                return Ok(_resp);
            }
            catch (Exception ex)
            {
                _resp.Message = ex.Message;
                return BadRequest(_resp);
            }

        }
        #endregion

        #region DeleteCoupon
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseDTO>> DeleteCoupon(int id)
        {
            try
            {
                if (id==0)
                {
                    _resp.Message = $"CouponId : {id} doesn't exist.";
                    return BadRequest(_resp);
                }
                Coupon data = await _db.Coupons.AsNoTracking().FirstAsync(u => u.CouponId == id);
                //checking for duplicate coupon
                if (data is null)
                {
                    _resp.Message = $"Coupon code: {id} doesn't exist. Please provide a valid coupon id.";
                    return BadRequest(_resp);
                }


                //mapping
                _db.Remove(data);
                await _db.SaveChangesAsync();
                _resp.IsSuccess = true;
                _resp.Message = $"Coupon: {data.CouponCode} deleted successfully";
                return Ok(_resp);
            }
            catch (Exception ex)
            {
                _resp.Message = ex.Message;
                return BadRequest(_resp);
            }
        }
        #endregion

    }
}
