using ScreenshotTranslatorApi.Services.Interfaces;

namespace ScreenshotTranslatorApi.Services;

/// <summary>
/// Mock implementation of ITranslationService for development and testing purposes
/// </summary>
public class MockTranslationService : ITranslationService
{
    private readonly ILogger<MockTranslationService> _logger;
    private readonly Dictionary<string, Dictionary<string, string>> _translations;

    public MockTranslationService(ILogger<MockTranslationService> logger)
    {
        _logger = logger;

        // Initialize predefined translations for common phrases
        _translations = new Dictionary<string, Dictionary<string, string>>
        {
            ["Hello world"] = new()
            {
                ["es"] = "Hola mundo",
                ["fr"] = "Bonjour le monde",
                ["de"] = "Hallo Welt",
                ["it"] = "Ciao mondo",
                ["ja"] = "こんにちは世界",
                ["zh"] = "你好，世界"
            },
            ["This is a test"] = new()
            {
                ["es"] = "Esto es una prueba",
                ["fr"] = "C'est un test",
                ["de"] = "Dies ist ein Test",
                ["it"] = "Questo è un test",
                ["ja"] = "これはテストです",
                ["zh"] = "这是一个测试"
            },
            ["Screenshot translation"] = new()
            {
                ["es"] = "Traducción de captura de pantalla",
                ["fr"] = "Traduction de capture d'écran",
                ["de"] = "Bildschirmfoto-Übersetzung",
                ["it"] = "Traduzione screenshot",
                ["ja"] = "スクリーンショット翻訳",
                ["zh"] = "截图翻译"
            },
            ["OpenAI integration"] = new()
            {
                ["es"] = "Integración de OpenAI",
                ["fr"] = "Intégration d'OpenAI",
                ["de"] = "OpenAI-Integration",
                ["it"] = "Integrazione OpenAI",
                ["ja"] = "OpenAI統合",
                ["zh"] = "OpenAI集成"
            },
            ["Azure Document Intelligence"] = new()
            {
                ["es"] = "Azure Document Intelligence",
                ["fr"] = "Azure Document Intelligence",
                ["de"] = "Azure Document Intelligence",
                ["it"] = "Azure Document Intelligence",
                ["ja"] = "Azure Document Intelligence",
                ["zh"] = "Azure Document Intelligence"
            },
            ["Semantic Kernel"] = new()
            {
                ["es"] = "Semantic Kernel",
                ["fr"] = "Semantic Kernel",
                ["de"] = "Semantic Kernel",
                ["it"] = "Semantic Kernel",
                ["ja"] = "Semantic Kernel",
                ["zh"] = "Semantic Kernel"
            },
            ["Image processing"] = new()
            {
                ["es"] = "Procesamiento de imágenes",
                ["fr"] = "Traitement d'image",
                ["de"] = "Bildverarbeitung",
                ["it"] = "Elaborazione delle immagini",
                ["ja"] = "画像処理",
                ["zh"] = "图像处理"
            },
            ["C# development"] = new()
            {
                ["es"] = "Desarrollo en C#",
                ["fr"] = "Développement C#",
                ["de"] = "C#-Entwicklung",
                ["it"] = "Sviluppo C#",
                ["ja"] = "C#開発",
                ["zh"] = "C#开发"
            },
            ["ASP.NET Core"] = new()
            {
                ["es"] = "ASP.NET Core",
                ["fr"] = "ASP.NET Core",
                ["de"] = "ASP.NET Core",
                ["it"] = "ASP.NET Core",
                ["ja"] = "ASP.NET Core",
                ["zh"] = "ASP.NET Core"
            },
            ["REST API"] = new()
            {
                ["es"] = "API REST",
                ["fr"] = "API REST",
                ["de"] = "REST-API",
                ["it"] = "API REST",
                ["ja"] = "REST API",
                ["zh"] = "REST API"
            }
        };
    }

    public Task<string> TranslateTextAsync(string sourceText, string targetLanguage, string? sourceLanguage = null)
    {
        _logger.LogInformation("{ServiceName} processing text to {targetLanguage}",
            nameof(MockTranslationService),
            targetLanguage);

        // Check if we have a predefined translation
        if (_translations.TryGetValue(sourceText, out var languageTranslations) &&
            languageTranslations.TryGetValue(targetLanguage, out var translation))
        {
            return Task.FromResult(translation);
        }

        // For any text without a predefined translation, add some prefix/suffix to simulate translation
        string simulatedTranslation;

        switch (targetLanguage.ToLower())
        {
            case "es":
                simulatedTranslation = $"ES: {sourceText} (translated)";
                break;
            case "fr":
                simulatedTranslation = $"FR: {sourceText} (traduit)";
                break;
            case "de":
                simulatedTranslation = $"DE: {sourceText} (übersetzt)";
                break;
            case "it":
                simulatedTranslation = $"IT: {sourceText} (tradotto)";
                break;
            case "ja":
                simulatedTranslation = $"JA: {sourceText} (翻訳済み)";
                break;
            case "zh":
                simulatedTranslation = $"ZH: {sourceText} (已翻译)";
                break;
            default:
                simulatedTranslation = $"{targetLanguage.ToUpper()}: {sourceText}";
                break;
        }

        return Task.FromResult(simulatedTranslation);
    }

    public async Task<string[]> TranslateTextsAsync(string[] sourceTexts, string targetLanguage, string? sourceLanguage = null)
    {
        _logger.LogInformation("Mock translation service processing {count} texts to {targetLanguage}",
            sourceTexts.Length, targetLanguage);

        var results = new List<string>();

        foreach (var text in sourceTexts)
        {
            var translatedText = await TranslateTextAsync(text, targetLanguage, sourceLanguage);
            results.Add(translatedText);
        }

        return [.. results];
    }
}