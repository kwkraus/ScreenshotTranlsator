namespace ScreenshotTranslatorApi.Models;

public class TranslationDetails
{
    /// <summary>
    /// The detected source language (if auto-detected)
    /// </summary>
    public string? DetectedSourceLanguage { get; set; }

    /// <summary>
    /// The target language the text was translated to
    /// </summary>
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Number of text elements detected by OCR
    /// </summary>
    public int DetectedElementCount { get; set; }

    /// <summary>
    /// Number of text elements that passed confidence threshold
    /// </summary>
    public int ProcessedElementCount { get; set; }

    /// <summary>
    /// List of text elements with their bounding boxes and translations
    /// </summary>
    public List<TextElement> Elements { get; set; } = new();

    /// <summary>
    /// Processing time metrics
    /// </summary>
    public ProcessingMetrics Metrics { get; set; } = new();
}

/// <summary>
/// Represents a single text element detected by OCR
/// </summary>
public class TextElement
{
    /// <summary>
    /// The original text as detected by OCR
    /// </summary>
    public string OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// The translated text
    /// </summary>
    public string TranslatedText { get; set; } = string.Empty;

    /// <summary>
    /// Confidence level of the OCR detection (0.0 to 1.0)
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// Bounding box coordinates of the text in the original image
    /// </summary>
    public BoundingBox BoundingBox { get; set; } = new();
}

/// <summary>
/// Represents a bounding box for text detected in an image
/// </summary>
public class BoundingBox
{
    /// <summary>
    /// X-coordinate of the top-left corner
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Y-coordinate of the top-left corner
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Width of the bounding box
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Height of the bounding box
    /// </summary>
    public int Height { get; set; }
}

/// <summary>
/// Performance metrics for the processing operations
/// </summary>
public class ProcessingMetrics
{
    /// <summary>
    /// Time taken for OCR processing in milliseconds
    /// </summary>
    public long OcrTimeMs { get; set; }

    /// <summary>
    /// Time taken for translation in milliseconds
    /// </summary>
    public long TranslationTimeMs { get; set; }

    /// <summary>
    /// Time taken for image overlay generation in milliseconds
    /// </summary>
    public long OverlayTimeMs { get; set; }

    /// <summary>
    /// Total processing time in milliseconds
    /// </summary>
    public long TotalProcessingTimeMs { get; set; }
}