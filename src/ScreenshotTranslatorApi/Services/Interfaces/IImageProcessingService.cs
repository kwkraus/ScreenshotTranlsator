using ScreenshotTranslatorApi.Models;

namespace ScreenshotTranslatorApi.Services.Interfaces;

/// <summary>
/// Service for processing and manipulating images
/// </summary>
public interface IImageProcessingService
{
    /// <summary>
    /// Overlays translated text on an image using the specified text elements and their bounding boxes
    /// </summary>
    /// <param name="base64Image">The original image in base64 format</param>
    /// <param name="textElements">List of text elements with original text, translated text, and bounding box</param>
    /// <returns>Base64-encoded string of the image with translated text overlayed</returns>
    Task<string> OverlayTranslatedTextAsync(string base64Image, List<TextElement> textElements);
}