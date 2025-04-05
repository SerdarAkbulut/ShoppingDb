using Microsoft.AspNetCore.Mvc;

namespace ShoppingApi.Controllers
{
    [ApiController]
    [Route("/api/error")]
    public class ErrorController : ControllerBase
    {
        [HttpGet("not-found")]
        public IActionResult NotFoundError()
        {
            return NotFound();
        }

        [HttpGet("bad-request")]
        public IActionResult BadRequestError()
        {
            return BadRequest();
        }
        [HttpGet("unauthhorized")]
        public IActionResult UnAuthorizedError()
        {
            return Unauthorized();
        }
        [HttpGet("server-error")]
        public IActionResult ServerError()
        {
            throw new Exception("server error");
        }
        [HttpGet("validation-error")]
        public IActionResult ValidationError()
        {
            ModelState.AddModelError("validation error","validation error details");
            return ValidationProblem();
        }
    }
}
