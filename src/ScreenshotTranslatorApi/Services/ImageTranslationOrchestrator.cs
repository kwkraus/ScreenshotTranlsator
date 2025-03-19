using Microsoft.SemanticKernel;
using ScreenshotTranslatorApi.Models;
using ScreenshotTranslatorApi.Services.Interfaces;
using System.Diagnostics;

namespace ScreenshotTranslatorApi.Services;

public class ImageTranslationOrchestrator(
    IOcrService ocrService,
    ITranslationService translationService,
    IImageProcessingService imageProcessingService,
    ILogger<ImageTranslationOrchestrator> logger)
{
    private readonly IOcrService _ocrService = ocrService;
    private readonly ITranslationService _translationService = translationService;
    private readonly IImageProcessingService _imageProcessingService = imageProcessingService;
    private readonly ILogger<ImageTranslationOrchestrator> _logger = logger;

    public async Task<ProcessScreenshotResponse> ProcessImageAsync(ProcessScreenshotRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var detailsBuilder = new TranslationDetails
        {
            TargetLanguage = request.TargetLanguage
        };

        try
        {
            _logger.LogInformation("Starting image translation workflow");

            // 1. Perform OCR using Document Intelligence
            var ocrStopwatch = Stopwatch.StartNew();
            var textElements = await _ocrService.RecognizeTextAsync(
                request.Image,
                request.FilterLowConfidenceResults ? request.MinConfidenceThreshold : 0);
            ocrStopwatch.Stop();

            _logger.LogInformation("OCR completed. Found {count} text elements", textElements.Count);

            // Update details
            detailsBuilder.DetectedElementCount = textElements.Count;
            detailsBuilder.ProcessedElementCount = textElements.Count;
            detailsBuilder.Metrics.OcrTimeMs = ocrStopwatch.ElapsedMilliseconds;

            if (textElements.Count == 0)
            {
                stopwatch.Stop();
                return new ProcessScreenshotResponse
                {
                    Status = "warning",
                    TranslatedText = "",
                    ImageWithOverlay = request.Image, // Return original image
                    ErrorMessage = "No text detected in the image",
                    Details = detailsBuilder
                };
            }

            // 2. Extract original text for translation
            var originalTexts = textElements.Select(e => e.OriginalText).ToArray();
            var allText = string.Join("\n", originalTexts);

            // 3. Translate text using Azure OpenAI
            var translationStopwatch = Stopwatch.StartNew();

            // If there are multiple text elements, use batch translation
            var translatedTexts = await _translationService.TranslateTextsAsync(
                originalTexts,
                request.TargetLanguage,
                request.SourceLanguage);

            translationStopwatch.Stop();
            detailsBuilder.Metrics.TranslationTimeMs = translationStopwatch.ElapsedMilliseconds;

            var translatedFullText = string.Join("\n", translatedTexts);
            _logger.LogInformation("Translation completed");

            // 4. Update text elements with translations
            for (int i = 0; i < textElements.Count && i < translatedTexts.Length; i++)
            {
                textElements[i].TranslatedText = translatedTexts[i];
            }

            // 5. Generate overlay using ImageProcessing
            var overlayStopwatch = Stopwatch.StartNew();
            var imageWithOverlay = await _imageProcessingService.OverlayTranslatedTextAsync(
                request.Image,
                textElements);
            overlayStopwatch.Stop();
            detailsBuilder.Metrics.OverlayTimeMs = overlayStopwatch.ElapsedMilliseconds;

            _logger.LogInformation("Image overlay completed");

            // 6. Prepare response
            stopwatch.Stop();
            detailsBuilder.Metrics.TotalProcessingTimeMs = stopwatch.ElapsedMilliseconds;
            detailsBuilder.Elements = textElements;

            return new ProcessScreenshotResponse
            {
                Status = "success",
                TranslatedText = translatedFullText,
                ImageWithOverlay = imageWithOverlay,
                Details = detailsBuilder
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error processing image translation");

            return new ProcessScreenshotResponse
            {
                Status = "error",
                ErrorMessage = $"Error processing image: {ex.Message}",
                Details = detailsBuilder
            };
        }
    }

    // Alternative implementation that uses Semantic Kernel
    // This is a simplified version without using the planner since we've run into compatibility issues
    public async Task<ProcessScreenshotResponse> ProcessImageWithSemanticKernelAsync(ProcessScreenshotRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var detailsBuilder = new TranslationDetails
        {
            TargetLanguage = request.TargetLanguage
        };

        try
        {
            _logger.LogInformation("Starting Semantic Kernel orchestrated image translation workflow");

            // Create Semantic Kernel builder
            var builder = Kernel.CreateBuilder();

            // Register our services as kernel functions
            var kernel = builder.Build();

            // Add our plugins (services wrapped as SK plugins)
            kernel.ImportPluginFromObject(new OcrPlugin(_ocrService), "OCR");
            kernel.ImportPluginFromObject(new TranslationPlugin(_translationService), "Translation");
            kernel.ImportPluginFromObject(new ImagePlugin(_imageProcessingService), "Image");

            // Setup the arguments dictionary
            var arguments = new KernelArguments
            {
                ["Image"] = request.Image,
                ["TargetLanguage"] = request.TargetLanguage,
                ["SourceLanguage"] = request.SourceLanguage,
                ["MinConfidence"] = request.MinConfidenceThreshold
            };

            // Since we're not using the planner in this simplified version,
            // call our direct implementation instead
            return await ProcessImageAsync(request);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error in Semantic Kernel workflow");

            return new ProcessScreenshotResponse
            {
                Status = "error",
                ErrorMessage = $"Error in Semantic Kernel workflow: {ex.Message}",
                Details = detailsBuilder
            };
        }
    }
}

// Sample plugins to be used by Semantic Kernel
public class OcrPlugin
{
    private readonly IOcrService _ocrService;

    public OcrPlugin(IOcrService ocrService)
    {
        _ocrService = ocrService;
    }

    [KernelFunction]
    public async Task<string> ExtractText(string image, float minConfidence = 0.6f)
    {
        var elements = await _ocrService.RecognizeTextAsync(image, minConfidence);
        return string.Join("\n", elements.Select(e => e.OriginalText));
    }

    [KernelFunction]
    public async Task<string> GetTextElements(string image, float minConfidence = 0.6f)
    {
        var elements = await _ocrService.RecognizeTextAsync(image, minConfidence);
        return System.Text.Json.JsonSerializer.Serialize(elements);
    }
}

public class TranslationPlugin
{
    private readonly ITranslationService _translationService;

    public TranslationPlugin(ITranslationService translationService)
    {
        _translationService = translationService;
    }

    [KernelFunction]
    public Task<string> TranslateText(string sourceText, string targetLanguage, string? sourceLanguage = null)
    {
        return _translationService.TranslateTextAsync(sourceText, targetLanguage, sourceLanguage);
    }

    [KernelFunction]
    public Task<string[]> TranslateTexts(string[] sourceTexts, string targetLanguage, string? sourceLanguage = null)
    {
        return _translationService.TranslateTextsAsync(sourceTexts, targetLanguage, sourceLanguage);
    }
}

public class ImagePlugin
{
    private readonly IImageProcessingService _imageProcessingService;

    public ImagePlugin(IImageProcessingService imageProcessingService)
    {
        _imageProcessingService = imageProcessingService;
    }

    [KernelFunction]
    public Task<string> OverlayText(string image, string textElementsJson)
    {
        var elements = System.Text.Json.JsonSerializer.Deserialize<List<TextElement>>(textElementsJson)
            ?? new List<TextElement>();
        return _imageProcessingService.OverlayTranslatedTextAsync(image, elements);
    }
}