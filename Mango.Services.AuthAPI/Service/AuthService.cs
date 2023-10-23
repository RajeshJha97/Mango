using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        #region PrivateVariable
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        #endregion

        #region Constructor
        public AuthService(ApplicationDbContext db,UserManager<ApplicationUser> userManager, RoleManager<IdentityRole>roleManager)
        {
           _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion

        #region LoginService
        public Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDto)
        {
            throw new NotImplementedException();
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
