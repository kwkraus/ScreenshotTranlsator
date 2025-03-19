using Azure;
using Azure.AI.DocumentIntelligence;
using ScreenshotTranslatorApi.Models;
using ScreenshotTranslatorApi.Services.Interfaces;

namespace ScreenshotTranslatorApi.Services;

public class AzureDocumentIntelligenceService : IOcrService
{
    private readonly DocumentIntelligenceClient _docIntelligenceClient;
    private readonly ILogger<AzureDocumentIntelligenceService> _logger;

    public AzureDocumentIntelligenceService(IConfiguration configuration, ILogger<AzureDocumentIntelligenceService> logger)
    {
        var endpoint = configuration["Azure:DocumentIntelligence:Endpoint"]
            ?? throw new ArgumentNullException("Azure:DocumentIntelligence:Endpoint not configured");
        var apiKey = configuration["Azure:DocumentIntelligence:ApiKey"]
            ?? throw new ArgumentNullException("Azure:DocumentIntelligence:ApiKey not configured");

        _docIntelligenceClient = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        _logger = logger;
    }

    public async Task<List<TextElement>> RecognizeTextAsync(string base64Image, float minConfidence = 0.6f)
    {
        try
        {
            _logger.LogInformation("Starting OCR processing with Azure Document Intelligence");

            // Convert base64 to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            using var imageStream = new MemoryStream(imageBytes);

            // Analyze document
            var options = new AnalyzeDocumentOptions("prebuild-read", new BinaryData(imageBytes));

            var operation = await _docIntelligenceClient.AnalyzeDocumentAsync(WaitUntil.Completed, options);

            var result = operation.Value;

            // Process text elements with bounding boxes
            var textElements = new List<TextElement>();

            foreach (var page in result.Pages)
            {
                foreach (var line in page.Lines)
                {
                    // Skip empty lines or lines with very low confidence
                    if (string.IsNullOrWhiteSpace(line.Content))
                        continue;

                    // Get the polygon points for the line's bounding box
                    var boundingBox = GetBoundingBoxFromPolygon(line.Polygon, page.Width, page.Height);

                    textElements.Add(new TextElement
                    {
                        OriginalText = line.Content,
                        Confidence = 1,
                        BoundingBox = boundingBox
                    });
                }
            }

            _logger.LogInformation("OCR processing completed. Found {count} text elements", textElements.Count);
            return textElements;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during OCR processing");
            throw;
        }
    }

    private BoundingBox GetBoundingBoxFromPolygon(IReadOnlyList<float> polygon, float? pageWidth, float? pageHeight)
    {
        // Calculate bounding box that encompasses all points in the polygon
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        // Polygon points are stored as [x1, y1, x2, y2, ...] in a flat array
        for (int i = 0; i < polygon.Count; i += 2)
        {
            if (i + 1 < polygon.Count)
            {
                float pointX = polygon[i];
                float pointY = polygon[i + 1];
                
                minX = Math.Min(minX, pointX);
                minY = Math.Min(minY, pointY);
                maxX = Math.Max(maxX, pointX);
                maxY = Math.Max(maxY, pointY);
            }
        }

        // Handle nullable values safely
        float safePageWidth = pageWidth ?? 1.0f;
        float safePageHeight = pageHeight ?? 1.0f;

        // Scale points to pixel values on the image
        int x = (int)(minX * safePageWidth);
        int y = (int)(minY * safePageHeight);
        int width = (int)((maxX - minX) * safePageWidth);
        int height = (int)((maxY - minY) * safePageHeight);

        return new BoundingBox
        {
            X = x,
            Y = y,
            Width = Math.Max(1, width),  // Ensure minimum of 1px
            Height = Math.Max(1, height) // Ensure minimum of 1px
        };
    }
}