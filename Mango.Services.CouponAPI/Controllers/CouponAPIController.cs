using Mango.Services.CouponAPI.Data;
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
        public CouponAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllCoupon()
        {
            var data = await _db.Coupons.ToListAsync();
            if (data == null)
            {
                return NotFound(new { StatusCode = HttpStatusCode.NotFound, Error = "No Data found" });
            }
            return Ok(new { StatusCode = HttpStatusCode.OK, RecordCount = data.Count, Data = data });
        }

        //[HttpGet("{id:int}",Name = "GetCouponByID")]
        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCouponByID(int id)
        {
            if (id == 0)
            {
                return NotFound(new { StatusCode = HttpStatusCode.NotFound, Error = "No Data found" });
            }
            var data = await _db.Coupons.FirstOrDefaultAsync(u=>u.CouponId==id);
            if (data == null)
            {
                return NotFound(new { StatusCode = HttpStatusCode.NotFound, Error = $"No Data found with id: {id}" });
            }
            return Ok(new { StatusCode = HttpStatusCode.OK, Data = data });
        }
    }
}
