using ScreenshotTranslatorApi.Models;
using ScreenshotTranslatorApi.Services.Interfaces;

namespace ScreenshotTranslatorApi.Services;

/// <summary>
/// Mock implementation of IOcrService for development and testing purposes
/// </summary>
public class MockOcrService : IOcrService
{
    private readonly ILogger<MockOcrService> _logger;
    private readonly Random _random = new();

    public MockOcrService(ILogger<MockOcrService> logger)
    {
        _logger = logger;
    }

    public Task<List<TextElement>> RecognizeTextAsync(string base64Image, float minConfidence = 0.6f)
    {
        _logger.LogInformation("{ServiceName} processing image", nameof(MockOcrService));

        // For test purposes, we'll create some random text elements with bounding boxes
        var textElements = new List<TextElement>();

        // Simulate finding 3-7 text elements in the image
        var elementCount = _random.Next(3, 8);

        // Create sample text elements
        var sampleTexts = new[]
        {
            "Hello world",
            "This is a test",
            "Screenshot translation",
            "OpenAI integration",
            "Azure Document Intelligence",
            "Semantic Kernel",
            "Image processing",
            "C# development",
            "ASP.NET Core",
            "REST API"
        };

        // Create mock text elements with bounding boxes
        for (int i = 0; i < elementCount; i++)
        {
            var text = sampleTexts[_random.Next(0, sampleTexts.Length)];
            var confidence = (float)(_random.NextDouble() * 0.3 + 0.7); // 0.7 - 1.0

            // Create a random bounding box (simulating the detected text location)
            var boundingBox = new BoundingBox
            {
                X = _random.Next(10, 400),
                Y = _random.Next(10, 300),
                Width = _random.Next(100, 300),
                Height = _random.Next(20, 50)
            };

            textElements.Add(new TextElement
            {
                OriginalText = text,
                Confidence = confidence,
                BoundingBox = boundingBox
            });
        }

        _logger.LogInformation("{ServiceName} detected {count} text elements", nameof(MockOcrService), textElements.Count);
        return Task.FromResult(textElements);
    }
}