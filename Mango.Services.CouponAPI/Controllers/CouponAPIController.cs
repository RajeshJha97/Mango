using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDTO _resp;
        public CouponAPIController(ApplicationDbContext db)
        {
            _db = db;
            _resp = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllCoupon()
        {
            try
            {
                var data = await _db.Coupons.ToListAsync();
                if (data == null)
                {
                    _resp.Message = "No Data found";
                    return NotFound(_resp);
                }
                _resp.Result = data;
                _resp.IsSuccess = true;
                _resp.Message = $"Number of records : {data.Count}";
                return Ok(_resp);
            }
            catch (Exception ex)
            {
                _resp.Message =ex.Message;
                return BadRequest(_resp);
            }
            
        }

        //[HttpGet("{id:int}",Name = "GetCouponByID")]
        [HttpGet]
        [Route("{id:int}")]
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
                var data = await _db.Coupons.FirstOrDefaultAsync(u => u.CouponId == id);
                if (data == null)
                {
                    _resp.Message = $"No Data found with id: {id}";
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
    }
}
