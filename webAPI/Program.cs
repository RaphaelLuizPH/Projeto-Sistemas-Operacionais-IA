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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://proud-rock-0f29bc20f.6.azurestaticapps.net")
            .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowCredentials();
    });


    options.AddPolicy("LocalHost", policy =>
   {
       policy.WithOrigins("http://localhost:5173")
             .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowCredentials();
   });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Only add Swagger in development environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "InvestigaIA API",
            Version = "v1",
            Description = "API for the InvestigaIA game"
        });
    });
}

builder.Services.AddControllers();


var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();

}
else
{

}

app.UseCors("AllowAll");
app.MapControllers();



app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();




app.MapHub<GameHub>("/gamehub");



app.Run();

