using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        #region PrivateVariables
        private readonly ResponseDTO _resp;
        private readonly IAuthService _auth;
        #endregion

        #region Constructor
        public AuthAPIController(IAuthService auth)
        {
            _resp = new ResponseDTO();
            _auth = auth;
        }
        #endregion

        #region Register
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            try 
            {
                if (registrationRequestDTO == null)
                {
                    _resp.Message = "Request body is null";
                    return BadRequest(_resp);
                }
                var errorMessage = await _auth.Register(registrationRequestDTO);
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    _resp.Message = errorMessage;
                    return BadRequest(_resp);
                }
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

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }
        #endregion
    }
}
