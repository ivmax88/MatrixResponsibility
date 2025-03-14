using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatrixResponsibility.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger=logger;
        }
        [HttpGet]
        public IActionResult Get()
        {
            logger.LogInformation("home test");
            return Ok("test");
        }
    }
}
