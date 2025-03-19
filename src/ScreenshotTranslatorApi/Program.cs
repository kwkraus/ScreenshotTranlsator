using ScreenshotTranslatorApi.Services;
using ScreenshotTranslatorApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add Azure service configuration
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure service injection
if (builder.Environment.IsDevelopment())
{
    // Use mock services for development and testing
    //builder.Services.AddScoped<IOcrService, MockOcrService>();
    //builder.Services.AddScoped<ITranslationService, MockTranslationService>();
    //builder.Services.AddScoped<IImageProcessingService, MockImageProcessingService>();

    builder.Services.AddScoped<IOcrService, AzureDocumentIntelligenceService>();
    builder.Services.AddScoped<ITranslationService, AzureOpenAITranslationService>();
    builder.Services.AddScoped<IImageProcessingService, ImageProcessingService>();

}
else
{
    // Use real Azure services in production
    builder.Services.AddScoped<IOcrService, AzureDocumentIntelligenceService>();
    builder.Services.AddScoped<ITranslationService, AzureOpenAITranslationService>();
    builder.Services.AddScoped<IImageProcessingService, ImageProcessingService>();
}

// Register the orchestrator
builder.Services.AddScoped<ImageTranslationOrchestrator>();

// Add CORS for client application
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowScreenshotApp", policy =>
    {
        policy.WithOrigins("http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowScreenshotApp");
app.UseAuthorization();

app.MapControllers();

app.Run();
