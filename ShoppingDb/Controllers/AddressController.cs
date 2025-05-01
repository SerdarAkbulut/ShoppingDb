using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;
using ShoppingApi.DTO;
using ShoppingApi.Entity;

namespace ShoppingApi.Controllers
{
    [ApiController]
    [Route("api/address")]
    public class AddressController : ControllerBase
    {
        private readonly DataContext _context;

        public AddressController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAddress()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var userAddresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();

          
            return Ok(userAddresses);
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddressDTO addressDTO)
        {
           var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var newAddress = new Address
            {
                Sehir = addressDTO.Sehir,
                Ilce = addressDTO.Ilce,
                FullAddress = addressDTO.FullAddress,
                Sokak = addressDTO.Sokak,
                UserId = userId,
                Cadde=addressDTO.Cadde,
                AdSoyad = addressDTO.AdSoyad,
                ApartmanNo = addressDTO.ApartmanNo,
                DaireNo = addressDTO.DaireNo,
                Phone = addressDTO.Phone

            };
            await _context.Addresses.AddAsync(newAddress);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Adres Kaydı başarılı" });
        }
    }
}