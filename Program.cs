using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// DbContext ve servisleri ekliyom
builder.Services.AddDbContext<AppDbContext>(options => 
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JSON Döngülerini (Cycle) önlemek için ayar
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHostedService<RuleMatchWorker>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

/*// Rule ekleme endpoint
app.MapPost("/rule-definitions", async (RuleDefinitionDto dto, AppDbContext db) =>
{
    var rule = new RuleDefinition
    {
        RuleName = dto.RuleName,
        Color = dto.Color,
        ShippingCountry = dto.ShippingCountry,
        ProductionCountry = dto.ProductionCountry,
        TestId = dto.TestId
    };

    db.RuleDefinitions.Add(rule);
    await db.SaveChangesAsync();

    return Results.Created($"/rule-definitions/{rule.RuleDefinitionId}", rule);
});

// Rule listeleme endpoint
app.MapGet("/rules", async (AppDbContext db) =>
{
    var rules = await db.RuleDefinition
        .Include(r => r.ProductTest)
        .ToListAsync();

    return Results.Ok(rules);
});

// Rule detay endpoint
app.MapGet("/rules/{id}", async (int id, AppDbContext db) =>
{
    var rule = await db.Rules
        .Include(r => r.Test)
        .FirstOrDefaultAsync(r => r.RuleId == id);

    return rule is not null ? Results.Ok(rule) : Results.NotFound();
});

*/

// Yeni kural tanımı ekleme
app.MapPost("/rule-definitions", async (RuleDefinitionDto dto, AppDbContext db) =>
{
    var rule = new RuleDefinition
    {
        RuleName = dto.RuleName,
        Color = dto.Color,
        ShippingCountry = dto.ShippingCountry,
        ProductionCountry = dto.ProductionCountry,
        ProductTestId = dto.ProductTestId
    };

    db.RuleDefinitions.Add(rule);
    await db.SaveChangesAsync();

    return Results.Created($"/rule-definitions/{rule.RuleDefinitionId}", rule);
});

// Tüm kural tanımlarını listeleme
app.MapGet("/rule-definitions", async (AppDbContext db) =>
{
    var rules = await db.RuleDefinitions
        .Include(r => r.ProductTest)
        .ToListAsync();

    return Results.Ok(rules);
});

// Belirli kural tanımını getirme
app.MapGet("/rule-definitions/{id}", async (int id, AppDbContext db) =>
{
    var rule = await db.RuleDefinitions
        .Include(r => r.ProductTest)
        .FirstOrDefaultAsync(r => r.RuleDefinitionId == id);

    return rule is not null ? Results.Ok(rule) : Results.NotFound();
});

// Eşleşmeleri listeleme
app.MapGet("/matches", async (AppDbContext db) =>
{
    var matches = await db.ProductRuleMatches
        .Include(m => m.Product)
        .Include(m => m.RuleDefinition)
        .ThenInclude(r => r.ProductTest)
        .ToListAsync();

    return Results.Ok(matches);
});

// Belirli ürün için eşleşmeleri getirme
app.MapGet("/matches/product/{productId}", async (int productId, AppDbContext db) =>
{
    var matches = await db.ProductRuleMatches
        .Where(m => m.ProductId == productId)
        .Include(m => m.RuleDefinition)
        .ThenInclude(r => r.ProductTest)
        .ToListAsync();

    return matches.Any() ? Results.Ok(matches) : Results.NotFound();
});

// Belirli kural için eşleşmeleri getirme
app.MapGet("/matches/rule/{ruleDefinitionId}", async (int ruleDefinitionId, AppDbContext db) =>
{
    var matches = await db.ProductRuleMatches
        .Where(m => m.RuleDefinitionId == ruleDefinitionId)
        .Include(m => m.Product)
        .ToListAsync();

    return matches.Any() ? Results.Ok(matches) : Results.NotFound();
});



app.Run();
