using Mango.Services.ProductAPI.Models.DTO;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = "Admin,MasterAdmin,Customer")]
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
        [AllowAnonymous]
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

        #region CreateProduct
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin,MasterAdmin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO req)
        {
            try
            {
                if (req != null)
                {
                    int resp=await _productService.CreateProduct(req);
                    if (resp >0) 
                    {
                        _resp.IsSuccess=true;
                        _resp.Message = $"{req.Name} created successfully";
                        return CreatedAtRoute("GetProductById",new { ProductId = resp.ToString() },_resp);
                    }
                    
                    _resp.Message = $"Product already exist with name : {req.Name}.";
                    return BadRequest(_resp);
                }
                _resp.Message = $"Please provide a valid request.";
                return BadRequest(_resp);
            }
            catch (Exception ex)
            {
                _resp.IsSuccess = false;
                _resp.Message = ex.Message;
                return BadRequest(_resp);
            }
        }
        #endregion

        #region UpdateProduct
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin,MasterAdmin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO req)
        {
            try
            {
                if (req != null)
                {
                    string resp = await _productService.UpdateProduct(req);
                    if (resp != null)
                    {
                        _resp.IsSuccess = true;
                        _resp.Message = resp;
                        return Ok(_resp);
                    }
                    _resp.Message = $"Product is not exist with the product Id : {req.ProductId}";
                    return NotFound(_resp);
                }
                _resp.Message = $"Please provide a valid request.";
                return BadRequest(_resp);
            }
            catch (Exception ex)
            {
                _resp.IsSuccess = false;
                _resp.Message = ex.Message;
                return BadRequest(_resp);
            }
        }
        #endregion

        #region DeleteProduct
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin,MasterAdmin")]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {
            try
            {
                if (id > 0)
                {
                    string resp = await _productService.DeleteProduct(id);
                    if (resp != null)
                    {
                        _resp.IsSuccess = true;
                        _resp.Message = resp;
                        return Ok(_resp);
                    }
                    _resp.Message = $"Product is not exist with the product Id : {id}";
                    return NotFound(_resp);
                }
                _resp.Message = $"Please provide a valid request.";
                return BadRequest(_resp);
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
