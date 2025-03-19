namespace ScreenshotTranslatorApi.Services.Interfaces;

/// <summary>
/// Service for translating text between languages
/// </summary>
public interface ITranslationService
{
    /// <summary>
    /// Translates a block of text from one language to another
    /// </summary>
    /// <param name="sourceText">Text to translate</param>
    /// <param name="targetLanguage">Language code to translate to (e.g., "en", "fr", "es")</param>
    /// <param name="sourceLanguage">Optional source language code (if not provided, will be auto-detected)</param>
    /// <returns>The translated text</returns>
    Task<string> TranslateTextAsync(string sourceText, string targetLanguage, string? sourceLanguage = null);

    /// <summary>
    /// Translates multiple text segments in a batch operation
    /// </summary>
    /// <param name="sourceTexts">Array of texts to translate</param>
    /// <param name="targetLanguage">Language code to translate to</param>
    /// <param name="sourceLanguage">Optional source language code</param>
    /// <returns>Array of translated texts in the same order</returns>
    Task<string[]> TranslateTextsAsync(string[] sourceTexts, string targetLanguage, string? sourceLanguage = null);
}