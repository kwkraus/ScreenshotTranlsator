# High-Level Blueprint

## Plan / Requirements Setup

- Validate the project’s specification and confirm the core tasks.
- Understand dependencies (OCR, translation APIs, .NET backend, Windows app framework).
- Decide on tools (Windows Credential Manager, local config file).

## Basic Application Framework

- Establish a .NET solution and a simple Windows desktop project.
- Implement a minimal UI (system tray icon in Windows with an about box).
- Add code to store and retrieve an API key from the Windows Credential Manager.

## Server-Side API

- Create a .NET web API to authorize the API key.
- Implement a stub for OCR and translation endpoints (return mock data initially).
- Add authentication logic.

## Screenshot Capture and OCR Integration

- Add system-wide hotkey listener.
- Implement screenshot capture region selection.
- Post the image to the server-side API for OCR.

## Translation and Overlay

- Add translation call after OCR.
- Render overlay of translated text on the captured screenshot.
- Enable copy of text or entire overlaid image to clipboard.

## Preferences and Settings

- Add UI to choose default translation language and AI model.
- Load / save user preferences in a local config file.
- Verify correct communication with the server.

## Registration and Installer

- Implement user registration flow that generates an API key.
- Store the generated key securely in the Windows Credential Manager.
- Package into an .exe installer with required libraries.

## Testing & Final Polishing

- Add unit tests for server endpoints and desktop side.
- Perform integration tests.
- Optimize for stability, finalize error handling, and user messages.

---

# 2. Break Into Smaller Iterative Chunks

## Initial App Foundation

- Create the .NET solution.
- Basic desktop app with minimal UI.
- System tray integration.

## API Skeleton

- Create .NET web API project.
- Add an `/authenticate` endpoint.
- Add a stub `/process-screenshot` endpoint returning mock translations.
- Security and Credential Management

## Integrate Windows Credential Manager into the desktop app.

- Implement a method to store/retrieve the API key.

## Screenshot Capture

- Implement a global hotkey.
- Capture user-selected region.
- Prepare data for transmission to API.

## OCR and Translation

- Connect the desktop app to the mock endpoint.
- Send screenshot data, receive mock OCR + translation.
- Display translation in a basic window.

## Overlay and Clipboard

- Render translation text overlayed on screenshot.
- Provide user with two copying options (text or image).

## Localization Settings

- Add UI elements for language/model selection.
- Save choices to a local config file.
- Apply the settings to the OCR/translation call.

## Registration and Installer

- Add registration form and call to POST `/authenticate`.
- Securely store newly created API key.
- Build .exe installer with everything bundled.

## Final Testing

- Write unit tests for each server controller.
- Write integration tests covering the entire user flow.
- Fix bugs, refine UI, finalize user messages.
- Confirm readiness for release.

---

# 3. Another Round of Breaking Those Chunks

Let’s further subdivide each chunk into actionable steps:

## Chunk 1: Initial App Foundation

- Create .NET solution folder.
- Use Visual Studio or CLI to scaffold a Windows desktop project (WPF or Windows Forms).
- Add a tray icon that shows a simple “Hello World” tooltip.
- Build & verify the tray icon loads on startup.

## Chunk 2: API Skeleton

- Scaffold a .NET web API project.
- Define `AuthenticateController` with a POST `/authenticate` endpoint returning `{ "status": "success", "message": "Stub authenticate" }`.
- Define `ProcessScreenshotController` with a POST `/process-screenshot` endpoint returning simple mock text.
- Test endpoints locally (console logs or Postman).

## Chunk 3: Security and Credential Management

- In the desktop app, create a helper class for reading/writing Windows Credentials.
- Implement `StoreApiKey(string key)` and `RetrieveApiKey()` methods.
- Add basic UI to input an API key for testing.
- Ensure the key is persisted in Windows Credential Manager.

## Chunk 4: Screenshot Capture

- Implement global hotkey registration.
- On hotkey press, dim screen and change cursor to crosshair.
- Capture selected region into a bitmap.
- Convert bitmap to base64 for later transmission.

## Chunk 5: OCR and Translation

- From the desktop app, send base64 screenshot to `/process-screenshot`.
- Return mock OCR text + translated text (to simulate).
- Display the translated text in a message box.
- Ensure errors and “no internet” scenarios are handled gracefully.

## Chunk 6: Overlay and Clipboard

- Add an in-memory image manipulation to draw text onto the screenshot.
- Render the final image in a picture box or temporary form.
- Add button for “Copy Text” and another for “Copy Overlaid Image.”
- Validate the output by pasting into a test application.

## Chunk 7: Localization Settings

- Create a Preferences dialog for language & model selection.
- Load / save these preferences to a local config file.
- Pass these settings in the `/process-screenshot` request.
- Confirm the preferences persist across restarts.

## Chunk 8: Registration and Installer

- Add registration form to gather user data.
- Issue a POST `/authenticate` request, store the returned API key.
- Configure .exe installer build.
- Smoke-test the installed program to confirm registration flow.

## Chunk 9: Final Testing

- Write unit tests for each server controller.
- Write integration tests covering the entire user flow.
- Fix bugs, refine UI, finalize user messages.
- Confirm readiness for release.

---

# 4. Step Sizing Verification

- Steps above are small enough to handle individually, yet each step is meaningful progress.
- Each step is testable, with minimal risk of large regressions.

---

# 5. Code-Generation LLM Prompts

Below is a series of prompts to guide a code-generation LLM, walking through each step. Each prompt is tagged as text with four backticks so they can be used verbatim.

## Prompt 1: Create the Desktop App Foundation

```text
[Task]
Create a new .NET Windows Forms application named "ScreenshotTranslatorApp" inside a solution called "ScreenshotTranslatorSolution". Add a tray icon that displays a tooltip "Screenshot Translator Running". On application start, the tray icon should appear in the system tray.

[Notes]
- Use .NET 6 if possible, or any LTS version.
- Show a minimal form on double-clicking the tray icon with a "Hello World" label inside.
- Provide any required instructions for setup in the project file or Program.cs comments.
```

## Prompt 2: Basic .NET Web API Skeleton

```text
[Task]
Inside the same solution, create a new .NET Web API project named "ScreenshotTranslatorApi". Add two controllers:
1. AuthenticateController with POST /authenticate returning { "status": "success", "message": "Stub authenticate" }.
2. ProcessScreenshotController with POST /process-screenshot returning { "status": "success", "translatedText": "Mock Translation", "imageWithOverlay": "" }.

[Notes]
- Use the standard .NET Web API template.
- Start the API on a different port than the desktop app.
- Confirm the controllers compile and run.
```

## Prompt 3: Windows Credential Manager Integration

```text
[Task]
In the "ScreenshotTranslatorApp" project, create a new static class "CredentialManagerHelper" that:
1. Uses Windows Credential Manager APIs to store a string named "ScreenshotTranslatorApiKey".
2. Retrieves the string from Credential Manager if it exists, otherwise returns null.

[Notes]
- Ensure we handle exceptions gracefully.
- Provide short usage instructions in the summary comments.
```

## Prompt 4: Global Hotkey and Region Capture

```text
[Task]
Add a global hotkey (e.g., Ctrl+Shift+S) to the "ScreenshotTranslatorApp". When pressed:
1. Dim the screen or create an overlay.
2. Change the cursor to a crosshair.
3. Let the user drag a rectangular selection.
4. Capture that region into a Bitmap object.

[Notes]
- Implement as reliably as possible on Windows.
- The code can rely on user32.dll or similar for hotkey registration.
- Return the Bitmap so further processing can happen.
```

## Prompt 5: Sending Screenshot to the API

```text
[Task]
When a screenshot is captured, convert it to a base64 string and POST it to /process-screenshot of the ScreenshotTranslatorApi. For now, just display the response in a message box.

[Notes]
- Use HttpClient for the call.
- Include a dummy JSON payload like { "image": "base64string", "outputLanguage": "en" }.
- Handle error scenarios with try/catch and show a message box.
```

## Prompt 6: Overlaying Text on the Screenshot

```text
[Task]
After receiving the response’s "translatedText", draw that text on the original Bitmap. Create a new window or form that shows the overlaid image. Add two buttons: "Copy Text" and "Copy Image". Implement them using Clipboard.SetText and Clipboard.SetImage.

[Notes]
- Make sure the text is clearly visible on the image (white text on black background, etc.).
- Keep it simple for now, no complex resizing or wrapping yet.
```

## Prompt 7: Preferences Dialog

```text
[Task]
Add a "Preferences" dialog in the desktop app, accessible from a right-click menu on the tray icon. The dialog should have:
1. A ComboBox to select a language (e.g., "en", "es", "fr").
2. A ComboBox for AI model selection (placeholder items "OpenAI" and "Anthropic").
3. A Save button that writes these settings to a local JSON config file (e.g., "userSettings.json").

[Notes]
- On app start, read the config if it exists, and populate the ComboBoxes.
- Later, these settings will be passed to the server in the screenshot process call.
```

## Prompt 8: Registration and API Key Storage

```text
[Task]
Create a registration form with basic fields: first name, last name, mobile, and email. On submission:
1. Call POST /authenticate, ignoring password or additional fields for now.
2. If status is "success", store the returned key using CredentialManagerHelper.
3. Hide the form and show the tray icon.

[Notes]
- For demonstration, the server can still return a stub success.
- The user interface can be minimal with basic textboxes and a submit button.
```

## Prompt 9: Installer and Final Testing

```text
[Task]
Set up a .NET installer project for the desktop app. Include:
1. The web API (if hosting locally) or instructions to host externally.
2. All necessary dependencies for screenshot capture and global hotkeys.
3. Clear instructions on how to install and run.

Finally, add basic unit tests for the API controllers and do a final pass for any missed items.

[Notes]
- For the unit tests, use MSTest or NUnit.
- The objective is to confirm the major functionalities are stable.
```