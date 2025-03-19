namespace ScreenshotTranslatorApi.Models;

public class ProcessScreenshotResponse
{
    /// <summary>
    /// Status of the processing request
    /// </summary>
    public string Status { get; set; } = "success";

    /// <summary>
    /// The translated text extracted from the image
    /// </summary>
    public string? TranslatedText { get; set; }

    /// <summary>
    /// The base64-encoded image with translated text overlayed
    /// </summary>
    public string? ImageWithOverlay { get; set; }

    /// <summary>
    /// Optional error message if processing failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Detailed information about the OCR and translation results
    /// </summary>
    public TranslationDetails? Details { get; set; }
}