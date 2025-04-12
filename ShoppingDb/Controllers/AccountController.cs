using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingApi.DTO;
using ShoppingApi.Entity;
using ShoppingApi.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApi.Controllers
{
    [ApiController]
    [Route("/api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
private readonly RoleManager<Role> _roleManager;
        public AccountController(UserManager<User> userManager, TokenService tokenService,RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Lütfen geçerli veriler girin." });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "E-posta adresi bulunamadı." });
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return Unauthorized(new { message = "Hesabınız kilitlenmiş, lütfen daha sonra tekrar deneyin." });
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return Unauthorized(new { message = "Geçersiz e-posta veya şifre." });
            }
var roles = await _userManager.GetRolesAsync(user);
var role=roles.FirstOrDefault();
            var token = await _tokenService.GenerateToken(user);
            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    role
                }
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult> CreateUser([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "Lütfen gerekli alanları doğru şekilde doldurun.",
                    errors = modelErrors
                });
            }


            var user = new User
            {
                
                Name=model.UserName,
                UserName = model.UserName,
                Email = model.Email
            };

           

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                return StatusCode(201, new
                {
                    success = true,
                    message = "Kullanıcı başarıyla oluşturuldu."
                });
            }

            // Identity hata mesajlarını çevirerek döndür
            var errorMessages = result.Errors.Select(e => new
            {
                code = e.Code,
                message = TranslateError(e.Code)
            }).ToList();

            return BadRequest(new
            {
                success = false,
                message = "Kullanıcı oluşturulamadı.",
                errors = errorMessages
            });
        }

        private string TranslateError(string errorCode)
        {
            return errorCode switch
            {
                "PasswordRequiresNonAlphanumeric" => "Şifre en az bir özel karakter içermelidir.",
                "PasswordRequiresDigit" => "Şifre en az bir rakam (0-9) içermelidir.",
                "PasswordRequiresUpper" => "Şifre en az bir büyük harf (A-Z) içermelidir.",
                "DuplicateUserName" => "Bu kullanıcı adı zaten alınmış.",
                "DuplicateEmail" => "Bu e-posta adresi zaten kullanılıyor.",
                "InvalidEmail" => "Geçersiz e-posta formatı.",
                "PasswordTooShort" => "Şifre çok kısa. Lütfen daha uzun bir şifre belirleyin.",
                _ => errorCode
            };
        }
    }
}
