using CredentialManagement;

namespace ScreenshotTranslatorApp;

/// <summary>
/// Helper class for managing API credentials in Windows Credential Manager.
/// 
/// Usage:
/// - To save: CredentialManagerHelper.SaveApiKey("your-api-key");
/// - To retrieve: string apiKey = CredentialManagerHelper.GetApiKey();
/// </summary>
public static class CredentialManagerHelper
{
    private const string CredentialTarget = "ScreenshotTranslatorApiKey";

    /// <summary>
    /// Saves the API key to Windows Credential Manager
    /// </summary>
    /// <param name="apiKey">The API key to save</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool SaveApiKey(string apiKey)
    {
        try
        {
            using var credential = new Credential
            {
                Target = CredentialTarget,
                Username = "ApiKey",
                Password = apiKey,
                Type = CredentialType.Generic,
                PersistanceType = PersistanceType.LocalComputer
            };

            return credential.Save();
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieves the API key from Windows Credential Manager
    /// </summary>
    /// <returns>The stored API key if found, null otherwise</returns>
    public static string? GetApiKey()
    {
        try
        {
            using var credential = new Credential { Target = CredentialTarget };
            if (!credential.Load())
                return null;

            return credential.Password;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Removes the API key from Windows Credential Manager
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public static bool DeleteApiKey()
    {
        try
        {
            using var credential = new Credential { Target = CredentialTarget };
            return credential.Delete();
        }
        catch (Exception)
        {
            return false;
        }
    }
}