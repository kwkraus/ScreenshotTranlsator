using ScreenshotTranslatorApi.Models;
using ScreenshotTranslatorApi.Services.Interfaces;

namespace ScreenshotTranslatorApi.Services;

/// <summary>
/// Mock implementation of IImageProcessingService for development and testing purposes
/// </summary>
public class MockImageProcessingService : IImageProcessingService
{
    private readonly ILogger<MockImageProcessingService> _logger;

    public MockImageProcessingService(ILogger<MockImageProcessingService> logger)
    {
        _logger = logger;
    }

    public Task<string> OverlayTranslatedTextAsync(string base64Image, List<TextElement> textElements)
    {
        _logger.LogInformation("Mock image processing service overlaying {count} text elements", textElements.Count);

        // In a real implementation, this would modify the image to overlay the translated text
        // For the mock implementation, we'll just return the original image

        // We could simulate changes by adding a marker in the string, but we'll keep it simple for now

        return Task.FromResult(base64Image);
    }
}