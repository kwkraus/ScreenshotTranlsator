using Microsoft.AspNetCore.Mvc;
using ScreenshotTranslatorApi.Models;
using ScreenshotTranslatorApi.Services;

namespace ScreenshotTranslatorApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProcessScreenshotController : ControllerBase
{
    private readonly ILogger<ProcessScreenshotController> _logger;
    private readonly ImageTranslationOrchestrator _orchestrator;

    public ProcessScreenshotController(
        ILogger<ProcessScreenshotController> logger,
        ImageTranslationOrchestrator orchestrator)
    {
        _logger = logger;
        _orchestrator = orchestrator;
    }

    [HttpPost("process-screenshot")]
    public async Task<ActionResult<ProcessScreenshotResponse>> ProcessScreenshot([FromBody] ProcessScreenshotRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Image))
            {
                return BadRequest(new ProcessScreenshotResponse
                {
                    Status = "error",
                    ErrorMessage = "Image is required"
                });
            }

            if (string.IsNullOrEmpty(request.TargetLanguage))
            {
                return BadRequest(new ProcessScreenshotResponse
                {
                    Status = "error",
                    ErrorMessage = "Target language is required"
                });
            }

            _logger.LogInformation("Processing screenshot translation request to {targetLanguage}",
                request.TargetLanguage);

            // Process the image using our orchestrator
            var response = await _orchestrator.ProcessImageAsync(request);

            if (response.Status == "error")
            {
                _logger.LogWarning("Error processing screenshot: {error}", response.ErrorMessage);
                return StatusCode(500, response);
            }

            if (response.Status == "warning")
            {
                _logger.LogWarning("Warning processing screenshot: {message}", response.ErrorMessage);
                return Ok(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in ProcessScreenshot");

            return StatusCode(500, new ProcessScreenshotResponse
            {
                Status = "error",
                ErrorMessage = "An unexpected error occurred while processing the screenshot"
            });
        }
    }

    // Optional Semantic Kernel version of the endpoint
    [HttpPost("process-screenshot-sk")]
    public async Task<ActionResult<ProcessScreenshotResponse>> ProcessScreenshotWithSK([FromBody] ProcessScreenshotRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Image))
            {
                return BadRequest(new ProcessScreenshotResponse
                {
                    Status = "error",
                    ErrorMessage = "Image is required"
                });
            }

            if (string.IsNullOrEmpty(request.TargetLanguage))
            {
                return BadRequest(new ProcessScreenshotResponse
                {
                    Status = "error",
                    ErrorMessage = "Target language is required"
                });
            }

            _logger.LogInformation("Processing screenshot with Semantic Kernel to {targetLanguage}",
                request.TargetLanguage);

            // Process the image using Semantic Kernel orchestration
            var response = await _orchestrator.ProcessImageWithSemanticKernelAsync(request);

            if (response.Status == "error")
            {
                _logger.LogWarning("Error processing screenshot with SK: {error}", response.ErrorMessage);
                return StatusCode(500, response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in ProcessScreenshotWithSK");

            return StatusCode(500, new ProcessScreenshotResponse
            {
                Status = "error",
                ErrorMessage = "An unexpected error occurred while processing the screenshot"
            });
        }
    }
}