using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ScreenshotTranslatorApi.Controllers;
using ScreenshotTranslatorApi.Models;
using ScreenshotTranslatorApi.Services;
using ScreenshotTranslatorApi.Services.Interfaces;

namespace ScreenshotTranslatorApi.Tests.Controllers;

public class ProcessScreenshotControllerTests
{
    private readonly Mock<ILogger<ProcessScreenshotController>> _loggerMock;
    private readonly Mock<ImageTranslationOrchestrator> _orchestratorMock;
    private readonly ProcessScreenshotController _controller;

    public ProcessScreenshotControllerTests()
    {
        _loggerMock = new Mock<ILogger<ProcessScreenshotController>>();
        _orchestratorMock = new Mock<ImageTranslationOrchestrator>(
            Mock.Of<IOcrService>(),
            Mock.Of<ITranslationService>(),
            Mock.Of<IImageProcessingService>(),
            Mock.Of<ILogger<ImageTranslationOrchestrator>>());

        _controller = new ProcessScreenshotController(_loggerMock.Object, _orchestratorMock.Object);
    }

    [Fact]
    public async Task ProcessScreenshot_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "base64string",
            TargetLanguage = "es"
        };

        var expectedResponse = new ProcessScreenshotResponse
        {
            Status = "success",
            TranslatedText = "Translated Text",
            ImageWithOverlay = "base64string"
        };

        _orchestratorMock.Setup(o => o.ProcessImageAsync(It.IsAny<ProcessScreenshotRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ProcessScreenshot(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ProcessScreenshotResponse>(okResult.Value);
        Assert.Equal("success", response.Status);
        Assert.Equal("Translated Text", response.TranslatedText);
        Assert.Equal("base64string", response.ImageWithOverlay);
    }

    [Fact]
    public async Task ProcessScreenshot_WithMissingImage_ReturnsBadRequest()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "",
            TargetLanguage = "es"
        };

        // Act
        var result = await _controller.ProcessScreenshot(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ProcessScreenshotResponse>(badRequestResult.Value);
        Assert.Equal("error", response.Status);
        Assert.Equal("Image is required", response.ErrorMessage);
    }

    [Fact]
    public async Task ProcessScreenshot_WithMissingTargetLanguage_ReturnsBadRequest()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "base64string",
            TargetLanguage = ""
        };

        // Act
        var result = await _controller.ProcessScreenshot(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ProcessScreenshotResponse>(badRequestResult.Value);
        Assert.Equal("error", response.Status);
        Assert.Equal("Target language is required", response.ErrorMessage);
    }

    [Fact]
    public async Task ProcessScreenshot_WithProcessingError_Returns500()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "base64string",
            TargetLanguage = "es"
        };

        var errorResponse = new ProcessScreenshotResponse
        {
            Status = "error",
            ErrorMessage = "Processing error"
        };

        _orchestratorMock.Setup(o => o.ProcessImageAsync(It.IsAny<ProcessScreenshotRequest>()))
            .ReturnsAsync(errorResponse);

        // Act
        var result = await _controller.ProcessScreenshot(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var response = Assert.IsType<ProcessScreenshotResponse>(statusCodeResult.Value);
        Assert.Equal("error", response.Status);
        Assert.Equal("Processing error", response.ErrorMessage);
    }

    [Fact]
    public async Task ProcessScreenshot_WithWarning_ReturnsOkWithWarning()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "base64string",
            TargetLanguage = "es"
        };

        var warningResponse = new ProcessScreenshotResponse
        {
            Status = "warning",
            ErrorMessage = "No text detected",
            ImageWithOverlay = "base64string"
        };

        _orchestratorMock.Setup(o => o.ProcessImageAsync(It.IsAny<ProcessScreenshotRequest>()))
            .ReturnsAsync(warningResponse);

        // Act
        var result = await _controller.ProcessScreenshot(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ProcessScreenshotResponse>(okResult.Value);
        Assert.Equal("warning", response.Status);
        Assert.Equal("No text detected", response.ErrorMessage);
    }

    [Fact]
    public async Task ProcessScreenshot_WithException_Returns500()
    {
        // Arrange
        var request = new ProcessScreenshotRequest
        {
            Image = "base64string",
            TargetLanguage = "es"
        };

        _orchestratorMock.Setup(o => o.ProcessImageAsync(It.IsAny<ProcessScreenshotRequest>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.ProcessScreenshot(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var response = Assert.IsType<ProcessScreenshotResponse>(statusCodeResult.Value);
        Assert.Equal("error", response.Status);
    }
}