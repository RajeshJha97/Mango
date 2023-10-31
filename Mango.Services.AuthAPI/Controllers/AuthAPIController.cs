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
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDto)
        {
            if (loginRequestDto == null)
            {
                _resp.Message = "Please add user name and password";
                return BadRequest(_resp);
            }
           var authResponse= await _auth.Login(loginRequestDto);
            if (authResponse.User == null || authResponse.Token == null)
            {
                _resp.Message = "Incorrect username and password";
                return BadRequest(_resp);
            }
            _resp.Result = authResponse;
            _resp.IsSuccess = true;
            _resp.Message = "Logged In";
            return Ok(_resp);
        }
        #endregion

        #region AssignRole
        [HttpPost]
        [Route("AssignRole")]
        public async Task<IActionResult> AssignRole(string email, string roleName)
        {
            if (email is null || roleName is null)
            {
                return BadRequest(new
                {
                    Message="Email and Role is mandatory"
                });
            }
            bool assignRole=await _auth.AssignRole(email, roleName);
            if (assignRole) 
            {
                return Ok(new 
                { 
                    Message=$"Email : {email} assigned with the role : {roleName}"
                });
            }
            return BadRequest(new 
            {
                Message =$"No User found with the Email: {email}"
            });
        }
        #endregion
    }
}
