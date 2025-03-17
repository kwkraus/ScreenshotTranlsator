## Development Blueprint

Below is a step-by-step plan to build this project:

1. Project Setup  
   - Initialize a .NET Core solution.  
   - Set up basic solution structure with folders for UI, server, and shared models.  

2. Registration and API Key Generation  
   - Create a user registration form in the UI.  
   - Implement server-side endpoints to store user details and generate an API key.  
   - Store the API key securely in Windows Credential Manager.  

3. Screenshot Capture and Keyboard Shortcut  
   - Establish a global keyboard hook to detect the screenshot shortcut.  
   - Implement click-and-drag region selection.  
   - Capture and encode the screenshot to be sent to the server API.  

4. OCR and Translation  
   - Implement OCR workflow in the server API.  
   - Integrate translation APIs (OpenAI, Anthropic, etc.).  
   - Return translated text and the image with overlays.  

5. Overlay Mechanism  
   - Receive overlayed image from server or generate overlay locally.  
   - Auto-adjust font size or spacing to accommodate text.  
   - Provide options to copy text or the new image to clipboard.  

6. System Tray and Preferences  
   - Implement a system tray icon for user preferences.  
   - Allow language selection, AI model selection, and other settings.  

7. Error Handling and Logging  
   - Capture no-internet or OCR-failure scenarios.  
   - Log errors locally for troubleshooting.  

8. Deployment and Installer  
   - Package the solution in a structured .exe installer.  
   - Integrate registration into the installation process.  
   - Provide usage instructions and keyboard shortcut details.  

## Iterative Chunks

1. Initialize .NET Core solution.  
2. Add minimal server and UI projects.  
3. Implement user registration and secure API key storage.  
4. Introduce screenshot capture feature with keyboard shortcut.  
5. Add a simple OCR function (stubbed or mock).  
6. Integrate translation service.  
7. Implement overlay logic.  
8. Provide system tray settings and preferences.  
9. Wrap with error handling.  
10. Build and package as an installer.  

## Further Breakdown

- **Chunk 1:** Basic .NET project scaffolding.  
- **Chunk 2:** User registration UI + server endpoint.  
- **Chunk 3:** Screenshot utility classes.  
- **Chunk 4:** OCR integration as a separate service.  
- **Chunk 5:** Translation service calls.  
- **Chunk 6:** Overlay and clipboard features.  
- **Chunk 7:** Final polishing with tray icon, logging, and error handling.  
- **Chunk 8:** Installer creation and final tests.  

Ensure each chunk is tested before moving to the next.  

## Final Series of Prompts

```text
Prompt 1:
"Generate a minimal .NET Core solution named 'ScreenshotTranslatorSolution' with separate projects for the UI and server. Include a basic README for setup."

Prompt 2:
"Create a simple user registration form in the UI project with fields (firstName, lastName, mobileNumber, email, password), and wire it up to a server endpoint that stores the data in memory."

Prompt 3:
"Implement a global keyboard hook in the UI project to detect a specific keypress for starting a screenshot capture. Provide a minimal Implementation for bounding-box selection."

Prompt 4:
"Add OCR logic to the server project using a stub service that returns predetermined text from an image."

Prompt 5:
"Integrate a translation API client in the server project. Let the user choose the translation language. Return translated text from the stubbed OCR result."

Prompt 6:
"In the UI project, display and overlay the translated text on the captured screenshot. Offer buttons to copy the text or copy the image."

Prompt 7:
"Add a system tray icon to the UI project that opens preferences. Let the user switch translation languages and toggle AI model choices."

Prompt 8:
"Implement error logging and graceful handling of no-internet or OCR failures. Provide a visible error message in the UI."

Prompt 9:
"Create a script to build and package the solution as a structured .exe installer, incorporating the registration process and basic usage instructions."
```