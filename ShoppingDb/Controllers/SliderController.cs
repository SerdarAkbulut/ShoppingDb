using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;
using ShoppingApi.DTO;
using ShoppingApi.Entity;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace ShoppingApi.Controllers
{
    [Route("api/slider")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        public SliderController( IConfiguration configuration, DataContext context)
        {
           
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("add-item")]
        public async Task<IActionResult> AddSliderItem([FromBody] SliderDTO sliderDTO)
        {
            var cloudinaryUrl = _configuration["Cloudinary:CloudinaryUrl"];
            var cloudinary = new Cloudinary(cloudinaryUrl);
            cloudinary.Api.Secure = true;

            var slidersToAdd = new List<Slider>();

            foreach (var img in sliderDTO.Images)
            {
                var base64Data = Regex.Match(img.ImageUrl, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                var bytes = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(bytes))
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription("image.jpg", stream),
                        UseFilename = true,
                        UniqueFilename = true,
                        Overwrite = false
                    };

                    var uploadResult = cloudinary.Upload(uploadParams);

                    var slider = new Slider
                    {
                        PublicId = uploadResult.PublicId,
                        ImageUrl = uploadResult.SecureUrl.ToString()
                    };

                    slidersToAdd.Add(slider);
                }
            }
            await _context.Sliders.AddRangeAsync(slidersToAdd);
            // slider'ları veritabanına kaydet
            await _context.SaveChangesAsync();

            return Ok(new { message = "Slider items uploaded successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetSliders()
        {
            var sliders = _context.Sliders;
            return Ok(sliders);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            var slider = await _context.Sliders.FirstOrDefaultAsync(i => i.Id == id);

            var cloudinaryUrl = _configuration["Cloudinary:CloudinaryUrl"];
            var cloudinary = new Cloudinary(cloudinaryUrl);
            cloudinary.Api.Secure = true;
            var deletionParams = new DeletionParams(slider.PublicId);
            var deletionResult = cloudinary.Destroy(deletionParams);
            _context.Sliders.Remove(slider);
            _context.SaveChangesAsync();
            return Ok("SSlider Silindi");
        }
    }
}
