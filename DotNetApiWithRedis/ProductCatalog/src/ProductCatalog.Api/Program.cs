using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using static ProductCatalogContext;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ProductCatalogContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
}
);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("redisConnectionString");
    options.InstanceName = "ProductCatalog:";
});

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("redisConnectionString")!);

var app = builder.Build();
app.MapOpenApi();
app.MapHealthChecks("/health");

app.MapGet("/products/{id:guid}", async (
    Guid id,
    ProductCatalogContext db,
    IDistributedCache cache,
    CancellationToken ct) =>
{
    var cacheKey = $"product:{id:N}";
    var cached = await cache.GetStringAsync(cacheKey, ct);
    if (cached is not null)
    {
        return Results.Ok(JsonSerializer.Deserialize<ProductResponse>(cached));
    }

    var product = await db.Products
    .AsNoTracking()
    .Where(p => p.Id == id)
    .Select(p => new ProductResponse
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
    })
    .SingleOrDefaultAsync(p => p.Id == id, ct);

    if (product is null)
    {
        return Results.NotFound();
    }

    await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product), new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    }, ct);

    return Results.Ok(product);
});

app.MapPost("/products", async (CreateProductRequest request, ProductCatalogContext db, CancellationToken ct) =>
{
    var product = new Product
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Price = request.Price
    };
    db.Products.Add(product);
    await db.SaveChangesAsync(ct);

    return Results.Created($"/products/{product.Id}", new ProductResponse
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price
    });
});

/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
*/

app.Run();