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
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });


    options.AddPolicy("AllowAll", policy =>
   {
       policy.WithOrigins("http://localhost:5173")
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
        Title = "Weather API",
        Version = "v1",
        Description = "A simple example ASP.NET Core Web API for weather forecasts"
    });
});
builder.Services.AddControllers();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

app.MapControllers();



app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.UseRouting();

app.MapHub<GameHub>("/gamehub");



app.Run();

