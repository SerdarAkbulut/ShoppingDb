using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;
using ShoppingApi.DTO;
using ShoppingApi.Entity;
using ShoppingApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace ShoppingApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
        private readonly DataContext _context;
        private readonly IEmailService _emailService;

        public UserController(UserManager<User> userManager, TokenService tokenService,DataContext context, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
            _emailService = emailService;
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserUpdateDTO updateUserDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

           
            // Şifre güncelleme yapılacaksa
            if (!string.IsNullOrEmpty(updateUserDTO.OldPassword) &&
                !string.IsNullOrEmpty(updateUserDTO.NewPassword) &&
                !string.IsNullOrEmpty(updateUserDTO.ConfirmPassword))
            {
                if (updateUserDTO.NewPassword != updateUserDTO.ConfirmPassword)
                {
                    return BadRequest(new { message = "Yeni şifre ve şifre tekrarı eşleşmiyor." });
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, updateUserDTO.OldPassword);
                if (!passwordCheck)
                {
                    var errors = new[]
                    {
        new
        {
            code = "InvalidOldPassword",
            message = TranslateError("Eski şifre eşleşmiyor")
        }
    };
                    return BadRequest(new { message = "Şifre güncellenemedi.", errors });
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, updateUserDTO.OldPassword, updateUserDTO.NewPassword);
                var errorMessages = passwordChangeResult.Errors.Select(e => new
                {
                    code = e.Code,
                    message = TranslateError(e.Code)
                }).ToList();
                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest(new { message = "Şifre güncellenemedi.", errors = errorMessages });
                }
            }

            // Diğer alanları güncelle
            if (!string.IsNullOrEmpty(updateUserDTO.Email))
                user.Email = updateUserDTO.Email;

            if (!string.IsNullOrEmpty(updateUserDTO.PhoneNumber))
                user.PhoneNumber = updateUserDTO.PhoneNumber;

            if (!string.IsNullOrEmpty(updateUserDTO.UserName))
                user.UserName = updateUserDTO.UserName;



            var result = await _userManager.UpdateAsync(user);
         
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Kullanıcı bilgileri güncellenemedi.", errors = result.Errors });
            }
            _context.SaveChangesAsync();

            return Ok(new { message = "Kullanıcı bilgileri başarıyla güncellendi." });
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await _userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }
            var userDetails = new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
            };
            return Ok(userDetails);

        }



        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null )
                return BadRequest("Kullanıcı bulunamadı.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var resetUrl = $"http://localhost:3000/reset-password?email={model.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(user.Email, "Şifre Sıfırlama",
                $"<p>Şifrenizi sıfırlamak için <a href='{resetUrl}'>buraya tıklayın</a>. Bu bağlantı 1 saat geçerlidir.</p>");

            return Ok("Şifre sıfırlama bağlantısı email adresinize gönderildi.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {


            if (model.NewPassword != model.ConfirmPassword)
                return BadRequest("Yeni şifre ve tekrarı uyuşmuyor.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Kullanıcı bulunamadı.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
            {
                var errorMessages = result.Errors.Select(e => new
                {
                    code = e.Code,
                    message = TranslateError(e.Code)
                }).ToList();

                return BadRequest(errorMessages);
            }

            return Ok(new { code = 200, message = "Şifre başarıyla güncellendi." });
        }
        [HttpGet("check-token")]
        public async Task<IActionResult> ChechToken([FromQuery] CheckTokenDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Kullanıcı bulunamadı.");


            var isValid = await _userManager.VerifyUserTokenAsync(
                 user,
                 TokenOptions.DefaultProvider, // "Default"
                 UserManager<User>.ResetPasswordTokenPurpose, // "ResetPassword"
                 model.Token);

            if (!isValid)
                return Ok(new{code=400, message="Token geçersiz veya süresi dolmuş."});
 
            return Ok(new { code = 200, message = "Token Geçerli" });

        }
        public string TranslateError(string errorCode)
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
