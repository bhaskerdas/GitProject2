using MyCorsApp.Models;
using MyCorsApp.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<ISeoAnalyzer, SeoAnalyzer>();
builder.Services.AddScoped<IRankingService, RankingService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapPost("/api/seo/analyze", async ([FromBody] string url, ISeoAnalyzer seoAnalyzer, IRankingService rankingService) =>
{
    if (string.IsNullOrWhiteSpace(url))
    {
        return Results.BadRequest("URL is required.");
    }

    // Simple URL validation
    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
    {
         return Results.BadRequest("Invalid URL format.");
    }

    var report = new SeoReport
    {
        Url = url,
        Ranking = await rankingService.GetRankingAsync(url),
        Issues = await seoAnalyzer.AnalyzeAsync(url)
    };

    // Simple recommendations based on issues
    foreach (var issue in report.Issues)
    {
        if (issue.Type == "Missing Title") report.Recommendations.Add("Add a <title> tag to your page.");
        if (issue.Type == "Missing Meta Description") report.Recommendations.Add("Add a meta description to improve click-through rates.");
        if (issue.Type == "Missing H1") report.Recommendations.Add("Add an <h1> tag for the main heading.");
        if (issue.Type == "Missing Image Alt") report.Recommendations.Add("Ensure all images have alt attributes for accessibility and SEO.");
    }

    return Results.Ok(report);
})
.WithName("AnalyzeSeo")
.WithOpenApi();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
