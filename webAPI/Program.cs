using InvestigaIA.API;
using InvestigaIA.Classes;
using webAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddSingleton<GameManager>();
builder.Services.AddSingleton<IServiceProvider, ServiceProvider>();
builder.Services.AddHttpClient<GeminiService>("GeminiClient", client =>
{
    var config = builder.Configuration;
    client.BaseAddress = new Uri(config["APIUrl"]);
});


builder.Services.AddHttpClient<OpenAiService>("OpenAIClient", client =>
{
    var config = builder.Configuration;
    client.BaseAddress = new Uri(config["APIUrl2"]);
    var apiKey = config["APIKey2"];
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

});


builder.Logging.AddConsole();




builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    string apiKey = config["APIKey"];


    return new GeminiService(APIKey: apiKey,
                             HttpClient: sp.GetRequiredService<IHttpClientFactory>().CreateClient("GeminiClient"));
});

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();


    return new OpenAiService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("OpenAIClient"));
});



builder.Services.AddHostedService<GameService>();

// Configure CORS - single configuration for all policies
builder.Services.AddCors(options =>
{
    // Production policy
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://proud-rock-0f29bc20f.6.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    // Local development policy
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    // Combined policy for multiple origins
    options.AddPolicy("CombinedPolicy", policy =>
    {
        policy.WithOrigins(
                "https://proud-rock-0f29bc20f.6.azurestaticapps.net",
                "http://localhost:5173"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "InvestigaIA API",
        Version = "v1",
        Description = "API for the InvestigaIA game"
    });
});


builder.Services.AddControllers();


var app = builder.Build();

// Apply middleware in the correct order
app.UseRouting();

// Apply CORS before any routing happens
app.UseCors("CombinedPolicy");

// Apply HTTPS redirection early in the pipeline
app.UseHttpsRedirection();

app.UseAuthorization();

// Development-specific middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// Map controllers and endpoints
app.MapControllers();
app.MapHub<GameHub>("/gamehub");

app.Run();

