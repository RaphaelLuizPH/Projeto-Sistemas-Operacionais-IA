using InvestigaIA.API;
using InvestigaIA.API.Gemini;
using InvestigaIA.Model.Game;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using webAPI.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR()
    .AddNewtonsoftJsonProtocol();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
          
            var accessToken = context.Request.Query["access_token"].ToString();
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/gamehub"))
            {
                
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };



});


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


builder.Services.AddSingleton<GameService>();

builder.Services.AddHostedService<GameCleanUpService>();
builder.Services.AddHostedService<GameAutoSave>();
builder.Services.AddHostedService<GameAutoLoad>();
builder.Services.AddSingleton(sp =>
{
    var config = builder.Configuration;
    string apiKey = config["APIKey"];


    return new GeminiService(APIKey: apiKey);
});

builder.Services.AddSingleton(sp =>
{


    return new OpenAiService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("OpenAIClient"));
});









builder.Services.AddCors(options =>
{
   
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://proud-rock-0f29bc20f.6.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });


    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    
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


builder.Services.AddControllers().AddNewtonsoftJson(); 


var app = builder.Build();


app.UseRouting();

app.UseCors("CombinedPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}


app.MapControllers();
app.MapHub<GameHub>("/gamehub");

app.Run();

