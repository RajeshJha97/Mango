using Mango.Services.ProductAPI.Models.DTO;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        #region PrivateMember
        private readonly IProductService _productService;
        private readonly Response _resp;
        #endregion

        #region Constructor
        public ProductAPIController(IProductService productService)
        {
            _productService = productService;
            _resp = new();
        }
        #endregion

        #region GetAllProducts
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response>> GetAllProducts()
        {
            try 
            {
                var products = await _productService.GetAllProducts();
                if (products == null)
                {
                    _resp.IsSuccess = false;
                    _resp.Message = "No product is avaliable";
                    return NotFound(_resp);
                }
                _resp.IsSuccess = true;
                _resp.Result = products;
                return Ok(_resp);
            }
            catch (Exception ex) 
            {
                _resp.IsSuccess = false;
                _resp.Message = ex.Message;
                return BadRequest(_resp);
            }
           
        }
        #endregion

        #region GetProductById
        [HttpGet]
        [Route("{productId:int}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response>> GetProductById(int productId)
        {
            try
            {
                if (productId <= 0)
                {
                    _resp.Message = $"Please provide a valid product id :{productId}";
                    return NotFound(_resp);
                }

                var product =await _productService.GetProductById(productId);
                if (product == null)
                {
                    _resp.Message = $"No product fetched with id :{productId}";                   
                    return NotFound(_resp);
                   
                }
                _resp.Result=product;
                _resp.IsSuccess = true;
                return Ok(_resp);
            }
            catch (Exception ex) 
            {
                _resp.IsSuccess = false;
                _resp.Message = ex.Message;
                return BadRequest(_resp);
            }
        }
        #endregion
    }
}
