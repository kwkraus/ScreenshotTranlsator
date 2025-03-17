using Microsoft.AspNetCore.Mvc;

namespace ScreenshotTranslatorApi.Controllers;

[ApiController]
[Route("")]
public class ProcessScreenshotController : ControllerBase
{
    private readonly ILogger<ProcessScreenshotController> _logger;

    public ProcessScreenshotController(ILogger<ProcessScreenshotController> logger)
    {
        _logger = logger;
    }

    [HttpPost("process-screenshot")]
    public IActionResult ProcessScreenshot()
    {
        var response = new { 
            status = "success", 
            translatedText = "Mock Translation", 
            imageWithOverlay = "" 
        };
        
        return Ok(response);
    }
}