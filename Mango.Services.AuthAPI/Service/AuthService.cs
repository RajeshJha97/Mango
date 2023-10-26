using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        #region PrivateVariable
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly ILogger<AuthService> _logger;

        #endregion

        #region Constructor
        public AuthService(ApplicationDbContext db,UserManager<ApplicationUser> userManager, RoleManager<IdentityRole>roleManager, 
            IJwtTokenGenerator tokenGenerator, ILogger<AuthService> logger)
        {
             _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }
        #endregion

        #region LoginService
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDto)
        {
            try
            {
                var user = await _db.ApplicationUsers.FirstAsync(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

                bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (user == null || isValid == false)
                {
                    return new LoginResponseDTO() { User = null, Token = "" };
                }

                UserDTO userDTO = new UserDTO()
                {
                    ID = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
                //if user found then generate jwt token
                string token = _tokenGenerator.GenerateToken(user);

                //token generation have to work
                return new LoginResponseDTO()
                {
                    User = userDTO,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception out: {ex.Message}");
                return new LoginResponseDTO()
                {
                    User = null,
                    Token = ""
                };
            }
          
        }

        #endregion

        #region RegisterService
        public async Task<string> Register(RegistrationRequestDTO registrationRequestDto)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber= registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.Email == registrationRequestDto.Email);
                    UserDTO userDTO = new UserDTO()
                    {
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        Email = userToReturn.Email,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    var err = result.Errors.FirstOrDefault().Description;
                    return err;
                }
                
            }
            catch (Exception ex)
            { 

            }
            return "error encountered";
        }
        #endregion
    }
}
