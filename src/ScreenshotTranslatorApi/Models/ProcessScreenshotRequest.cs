namespace ScreenshotTranslatorApi.Models;

public class ProcessScreenshotRequest
{
    /// <summary>
    /// The base64-encoded bitmap image to process
    /// </summary>
    public required string Image { get; set; }

    /// <summary>
    /// The target language code to translate the text to
    /// </summary>
    public required string TargetLanguage { get; set; }

    /// <summary>
    /// Optional source language code. If not provided, the API will attempt to detect the language.
    /// </summary>
    public string? SourceLanguage { get; set; }

    /// <summary>
    /// Optional parameter to indicate whether to perform OCR confidence filtering
    /// </summary>
    public bool FilterLowConfidenceResults { get; set; } = true;

    /// <summary>
    /// Optional minimum confidence threshold for OCR results (0.0 to 1.0)
    /// </summary>
    public float MinConfidenceThreshold { get; set; } = 0.6f;
}