using Microsoft.AspNetCore.Mvc;

namespace ScreenshotTranslatorApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly ILogger<AuthenticateController> _logger;

    public AuthenticateController(ILogger<AuthenticateController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Post()
    {
        var response = new { 
            status = "success", 
            message = "Stub authenticate" 
        };
        
        return Ok(response);
    }
}