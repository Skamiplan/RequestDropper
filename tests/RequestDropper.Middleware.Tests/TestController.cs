using Microsoft.AspNetCore.Mvc;

namespace RequestDropper.Middleware.Tests
{
    [ApiController]
    [Route("")]
    public class TestController : ControllerBase
    {
        [HttpGet("{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            return StatusCode(statusCode);
        } 
    }
}
