using Microsoft.Extensions.Logging;
using Moq;
using ScreenshotTranslatorApi.Models;
using ScreenshotTranslatorApi.Services;
using ScreenshotTranslatorApi.Services.Interfaces;

namespace ScreenshotTranslatorApi.Tests.Services;

public class ImageTranslationOrchestratorTests
{
    private readonly Mock<IOcrService> _ocrServiceMock;
    private readonly Mock<ITranslationService> _translationServiceMock;
    private readonly Mock<IImageProcessingService> _imageProcessingServiceMock;
    private readonly Mock<ILogger<ImageTranslationOrchestrator>> _loggerMock;
    private readonly ImageTranslationOrchestrator _orchestrator;

    public ImageTranslationOrchestratorTests()
    {
        _ocrServiceMock = new Mock<IOcrService>();
        _translationServiceMock = new Mock<ITranslationService>();
        _imageProcessingServiceMock = new Mock<IImageProcessingService>();
        _loggerMock = new Mock<ILogger<ImageTranslationOrchestrator>>();

        _orchestrator = new ImageTranslationOrchestrator(
            _ocrServiceMock.Object,
            _translationServiceMock.Object,
            _imageProcessingServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ProcessImageAsync_WithTextDetected_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "base64image",
            TargetLanguage = "fr",
            SourceLanguage = "en",
            FilterLowConfidenceResults = true,
            MinConfidenceThreshold = 0.7f
        };

        var textElements = new List<TextElement>
        {
            new TextElement
            {
                OriginalText = "Hello world",
                Confidence = 0.95f,
                BoundingBox = new BoundingBox { X = 10, Y = 10, Width = 100, Height = 20 }
            }
        };

        _ocrServiceMock.Setup(o => o.RecognizeTextAsync(request.Image, request.MinConfidenceThreshold))
            .ReturnsAsync(textElements);

        var translatedTexts = new[] { "Bonjour le monde" };

        _translationServiceMock.Setup(t => t.TranslateTextsAsync(
                It.Is<string[]>(arr => arr.Length == 1 && arr[0] == "Hello world"),
                request.TargetLanguage,
                request.SourceLanguage))
            .ReturnsAsync(translatedTexts);

        const string overlayedImage = "base64overlayed";
        _imageProcessingServiceMock.Setup(i => i.OverlayTranslatedTextAsync(
                request.Image,
                It.Is<List<TextElement>>(elements =>
                    elements.Count == 1 &&
                    elements[0].OriginalText == "Hello world" &&
                    elements[0].TranslatedText == "Bonjour le monde")))
            .ReturnsAsync(overlayedImage);

        // Act
        var result = await _orchestrator.ProcessImageAsync(request);

        // Assert
        Assert.Equal("success", result.Status);
        Assert.Equal("Bonjour le monde", result.TranslatedText);
        Assert.Equal(overlayedImage, result.ImageWithOverlay);
        Assert.NotNull(result.Details);
        Assert.Equal(1, result.Details!.DetectedElementCount);
        Assert.Equal(1, result.Details.ProcessedElementCount);
        Assert.Equal("fr", result.Details.TargetLanguage);
    }

    [Fact]
    public async Task ProcessImageAsync_WithNoTextDetected_ReturnsWarningResponse()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "base64image",
            TargetLanguage = "fr"
        };

        _ocrServiceMock.Setup(o => o.RecognizeTextAsync(request.Image, It.IsAny<float>()))
            .ReturnsAsync(new List<TextElement>());

        // Act
        var result = await _orchestrator.ProcessImageAsync(request);

        // Assert
        Assert.Equal("warning", result.Status);
        Assert.Equal("No text detected in the image", result.ErrorMessage);
        Assert.Equal(request.Image, result.ImageWithOverlay);
        Assert.NotNull(result.Details);
        Assert.Equal(0, result.Details!.DetectedElementCount);
        Assert.Equal(0, result.Details.ProcessedElementCount);
    }

    [Fact]
    public async Task ProcessImageAsync_WithOcrError_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "base64image",
            TargetLanguage = "fr"
        };

        _ocrServiceMock.Setup(o => o.RecognizeTextAsync(request.Image, It.IsAny<float>()))
            .ThrowsAsync(new Exception("OCR service error"));

        // Act
        var result = await _orchestrator.ProcessImageAsync(request);

        // Assert
        Assert.Equal("error", result.Status);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("OCR service error", result.ErrorMessage!);
    }

    [Fact]
    public async Task ProcessImageAsync_WithTranslationError_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "base64image",
            TargetLanguage = "fr"
        };

        var textElements = new List<TextElement>
        {
            new TextElement
            {
                OriginalText = "Hello world",
                Confidence = 0.95f,
                BoundingBox = new BoundingBox { X = 10, Y = 10, Width = 100, Height = 20 }
            }
        };

        _ocrServiceMock.Setup(o => o.RecognizeTextAsync(request.Image, It.IsAny<float>()))
            .ReturnsAsync(textElements);

        _translationServiceMock.Setup(t => t.TranslateTextsAsync(
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(new Exception("Translation service error"));

        // Act
        var result = await _orchestrator.ProcessImageAsync(request);

        // Assert
        Assert.Equal("error", result.Status);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Translation service error", result.ErrorMessage!);
    }
}