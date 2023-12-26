using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/Cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDTO _resp;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;

        public CartAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _resp = new();
            _db = db;
            _mapper = mapper;
        }

        [HttpPost("CartUpsert")]
        public async Task<ActionResult<ResponseDTO>> Upsert(CartDTO cartDto)
        {
            try
            {
                //We will check whether the cart header exist for the particular user
                var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //We have to create Header
                    var cartHeader=_mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();

                }
                else 
                {
                    //if header is not null
                    //Check if Details have same product and also that product must belong to the same user

                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync
                        (u=>u.ProductId==cartDto.CartDetails.First().ProductId 
                        && u.CartHeaderId==cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //we have to create new cart details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else 
                    {
                        //we have to Update the count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId= cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId=cartDetailsFromDb.CartHeaderId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync() ;

                    }

                }
                _resp.Result = cartDto;
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
