using Microsoft.AspNetCore.Mvc;

namespace RequestDropper.Middleware.Tests
{
    [ApiController]
    [Route("")]
    public class TestController : ControllerBase
    {
        [HttpGet("index/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            return StatusCode(statusCode);
        }

        [HttpGet("home/{statusCode}")]
        public IActionResult Home(int statusCode)
        {
            return StatusCode(statusCode);
        }
    }
}
