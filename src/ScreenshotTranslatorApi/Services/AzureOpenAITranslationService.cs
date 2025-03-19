using Azure;
using Azure.AI.OpenAI;
using ScreenshotTranslatorApi.Services.Interfaces;
using System.Text;

namespace ScreenshotTranslatorApi.Services;

public class AzureOpenAITranslationService : ITranslationService
{
    private readonly OpenAIClient _openAIClient;
    private readonly string _deploymentName;
    private readonly ILogger<AzureOpenAITranslationService> _logger;

    public AzureOpenAITranslationService(IConfiguration configuration, ILogger<AzureOpenAITranslationService> logger)
    {
        var endpoint = configuration["Azure:OpenAI:Endpoint"]
            ?? throw new ArgumentNullException("Azure:OpenAI:Endpoint not configured");
        var apiKey = configuration["Azure:OpenAI:ApiKey"]
            ?? throw new ArgumentNullException("Azure:OpenAI:ApiKey not configured");
        _deploymentName = configuration["Azure:OpenAI:DeploymentName"]
            ?? throw new ArgumentNullException("Azure:OpenAI:DeploymentName not configured");

        _openAIClient = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        _logger = logger;
    }

    public async Task<string> TranslateTextAsync(string sourceText, string targetLanguage, string? sourceLanguage = null)
    {
        try
        {
            _logger.LogInformation("Translating text to {targetLanguage}", targetLanguage);

            var options = new ChatCompletionsOptions
            {
                DeploymentName = _deploymentName,
                Messages =
                {
                    new ChatRequestSystemMessage(GetSystemPrompt(targetLanguage, sourceLanguage)),
                    new ChatRequestUserMessage(sourceText)
                },
                Temperature = 0.3f,
                MaxTokens = 2000
            };

            var response = await _openAIClient.GetChatCompletionsAsync(options);
            var translatedText = response.Value.Choices[0].Message.Content;

            _logger.LogInformation("Translation completed successfully");
            return translatedText ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during translation");
            throw;
        }
    }

    public async Task<string[]> TranslateTextsAsync(string[] sourceTexts, string targetLanguage, string? sourceLanguage = null)
    {
        try
        {
            if (sourceTexts.Length == 0)
                return [];

            if (sourceTexts.Length == 1)
                return [await TranslateTextAsync(sourceTexts[0], targetLanguage, sourceLanguage)];

            _logger.LogInformation("Batch translating {count} text segments to {targetLanguage}",
                sourceTexts.Length, targetLanguage);

            // Concatenate texts with a special delimiter that's unlikely to appear in the text
            const string delimiter = "|||SEGMENT_DELIMITER|||";
            var combinedText = string.Join(delimiter, sourceTexts);

            var options = new ChatCompletionsOptions
            {
                Messages =
                {
                    new ChatRequestSystemMessage(GetBatchTranslationPrompt(targetLanguage, sourceLanguage, delimiter)),
                    new ChatRequestUserMessage(combinedText)
                },
                Temperature = 0.3f,
                MaxTokens = 4000
            };

            var response = await _openAIClient.GetChatCompletionsAsync(options);
            var translatedCombined = response.Value.Choices[0].Message.Content ?? string.Empty;

            // Split the translated text back into segments
            var translatedTexts = translatedCombined.Split(delimiter, StringSplitOptions.None);

            // If we don't get the same number of segments back, do individual translations
            if (translatedTexts.Length != sourceTexts.Length)
            {
                _logger.LogWarning("Batch translation returned incorrect number of segments. " +
                                 "Expected: {expected}, Actual: {actual}. Falling back to individual translations.",
                    sourceTexts.Length, translatedTexts.Length);

                var results = new List<string>();
                foreach (var text in sourceTexts)
                {
                    results.Add(await TranslateTextAsync(text, targetLanguage, sourceLanguage));
                }
                return results.ToArray();
            }

            _logger.LogInformation("Batch translation completed successfully");
            return translatedTexts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during batch translation");
            throw;
        }
    }

    private string GetSystemPrompt(string targetLanguage, string? sourceLanguage)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"You are a professional translator.");

        if (sourceLanguage != null)
        {
            sb.AppendLine($"Translate the following text from {sourceLanguage} to {targetLanguage}.");
        }
        else
        {
            sb.AppendLine($"Translate the following text to {targetLanguage}.");
            sb.AppendLine("First detect the source language, then translate to the target language.");
        }

        sb.AppendLine("Keep the formatting and structure as close as possible to the original.");
        sb.AppendLine("Translate only the text, don't add any comments or explanations.");
        return sb.ToString();
    }

    private string GetBatchTranslationPrompt(string targetLanguage, string? sourceLanguage, string delimiter)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"You are a professional translator.");

        if (sourceLanguage != null)
        {
            sb.AppendLine($"Translate the following text segments from {sourceLanguage} to {targetLanguage}.");
        }
        else
        {
            sb.AppendLine($"Translate the following text segments to {targetLanguage}.");
        }

        sb.AppendLine($"The text segments are separated by the delimiter: {delimiter}");
        sb.AppendLine("Translate each segment separately, and keep them separated with the exact same delimiter.");
        sb.AppendLine("You must return exactly the same number of segments as in the input.");
        sb.AppendLine("Keep the formatting and structure as close as possible to the original.");
        sb.AppendLine("Translate only the text, don't add any comments or explanations.");
        return sb.ToString();
    }
}