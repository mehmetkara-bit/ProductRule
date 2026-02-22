using ProductRule.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

public class RuleMatchWorker : BackgroundService {
    private readonly IServiceProvider _services;

    public RuleMatchWorker(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            using (var scope = _services.CreateScope()) {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var rules = await db.Rules.ToListAsync();
                var products = await db.Products.Include(p => p.Detail).ToListAsync();

                var matches = new ConcurrentBag<ProductRuleMatch>();

                // Multithreading burada başlıyor
                await Parallel.ForEachAsync(products, new ParallelOptions { 
                    MaxDegreeOfParallelism = Environment.ProcessorCount // Çekirdek sayısı kadar thread
                }, (product, token) => {
                    
                    foreach (var rule in rules) {
                        if (IsMatch(product, rule)) {
                            // Eşleşme bulundu, kaydedelim
                            matches.Add(new ProductRuleMatch { ProductNo = product.ProductNo, RuleDefinitionId = rule.Id, MatchedAt = DateTime.Now });
                        }
                    }
                    return ValueTask.CompletedTask;
                });

                if (!matches.IsEmpty)
                {
                    await db.ProductRuleMatches.AddRangeAsync(matches);
                    await db.SaveChangesAsync();
                }
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // 1 saatte bir çalış
        }
    }

    private bool IsMatch(Product p, RuleDefinition r) {
        return (string.IsNullOrEmpty(r.Color) || p.Detail?.Color == r.Color) &&
               (string.IsNullOrEmpty(r.ShippingCountry) || p.Detail?.ShippingCountry == r.ShippingCountry) &&
               (string.IsNullOrEmpty(r.ProductionCountry) || p.ProductionCountry == r.ProductionCountry);
    }
}