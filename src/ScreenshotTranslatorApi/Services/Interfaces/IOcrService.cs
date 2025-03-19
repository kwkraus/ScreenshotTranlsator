using ScreenshotTranslatorApi.Models;

namespace ScreenshotTranslatorApi.Services.Interfaces;

/// <summary>
/// Service for performing OCR (Optical Character Recognition) on images
/// </summary>
public interface IOcrService
{
    /// <summary>
    /// Performs OCR on a base64-encoded image
    /// </summary>
    /// <param name="base64Image">The image in base64 format</param>
    /// <param name="minConfidence">Minimum confidence threshold for results</param>
    /// <returns>A list of text elements with their bounding boxes</returns>
    Task<List<TextElement>> RecognizeTextAsync(string base64Image, float minConfidence = 0.6f);
}