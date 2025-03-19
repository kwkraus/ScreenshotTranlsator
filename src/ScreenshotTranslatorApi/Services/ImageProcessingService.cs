using ScreenshotTranslatorApi.Models;
using ScreenshotTranslatorApi.Services.Interfaces;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using PointF = SixLabors.ImageSharp.PointF;

namespace ScreenshotTranslatorApi.Services;

public class ImageProcessingService : IImageProcessingService
{
    private readonly ILogger<ImageProcessingService> _logger;
    private readonly FontCollection _fontCollection;

    public ImageProcessingService(ILogger<ImageProcessingService> logger)
    {
        _logger = logger;
        _fontCollection = new FontCollection();

        // Add system fonts
        _fontCollection.AddSystemFonts();
    }

    public async Task<string> OverlayTranslatedTextAsync(string base64Image, List<TextElement> textElements)
    {
        try
        {
            _logger.LogInformation("Starting to overlay translated text on image");

            // Convert base64 to image
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            using var memoryStream = new MemoryStream(imageBytes);
            using var image = await Image.LoadAsync(memoryStream);

            // Prepare for text rendering
            var defaultFont = _fontCollection.Get("Arial").CreateFont(12);
            var textOptions = new TextOptions(defaultFont)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Origin = new PointF(0, 0)
            };

            // Process each text element
            foreach (var element in textElements)
            {
                if (string.IsNullOrWhiteSpace(element.TranslatedText))
                    continue;

                var box = element.BoundingBox;

                // Skip invalid bounding boxes
                if (box.Width <= 0 || box.Height <= 0 ||
                    box.X < 0 || box.Y < 0 ||
                    box.X + box.Width > image.Width ||
                    box.Y + box.Height > image.Height)
                {
                    _logger.LogWarning("Skipping invalid bounding box: X={X}, Y={Y}, Width={Width}, Height={Height}",
                        box.X, box.Y, box.Width, box.Height);
                    continue;
                }

                // Calculate font size based on available height
                float fontSize = CalculateOptimalFontSize(
                    element.TranslatedText,
                    box.Width,
                    box.Height);

                var font = _fontCollection.Get("Arial").CreateFont(fontSize);

                // Create a background rect to replace original text
                var bgRect = new RectangleF(box.X, box.Y, box.Width, box.Height);

                // Draw over the original text with a white background
                image.Mutate(ctx => ctx
                    .Fill(Color.White, bgRect)
                    .DrawText(new RichTextOptions(font)
                    {
                        Origin = new PointF(box.X, box.Y),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        WrappingLength = box.Width,
                        LineSpacing = 1.0f
                    }, element.TranslatedText, Color.Black));
            }

            // Convert back to base64
            using var outputStream = new MemoryStream();
            await image.SaveAsPngAsync(outputStream);
            var outputBase64 = Convert.ToBase64String(outputStream.ToArray());

            _logger.LogInformation("Successfully overlaid translated text on image");
            return outputBase64;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while overlaying translated text");
            throw;
        }
    }

    private float CalculateOptimalFontSize(string text, int boxWidth, int boxHeight, float maxFontSize = 72.0f)
    {
        // Start with a reasonable size
        float fontSize = 12.0f;

        // Set maximum size based on box height to avoid text being too large
        float maximumSize = Math.Min(maxFontSize, boxHeight * 0.8f);

        // Prevent oversized text
        if (maximumSize < 6.0f)
            return 6.0f;  // Minimum readable size

        // Start with small font and increase until it doesn't fit
        while (fontSize < maximumSize)
        {
            var testFont = _fontCollection.Get("Arial").CreateFont(fontSize + 1);
            var size = TextMeasurer.MeasureSize(text, new TextOptions(testFont)
            {
                WrappingLength = boxWidth
            });

            // If the text is too tall or too wide, stop increasing
            if (size.Height > boxHeight * 0.9f || size.Width > boxWidth * 0.95f)
                break;

            fontSize += 1;
        }

        return Math.Max(6.0f, fontSize); // Ensure at least 6pt font for readability
    }
}