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
            try
            {
                using (var scope = _services.CreateScope()) {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    
                    var rules = await db.RuleDefinitions.ToListAsync(stoppingToken);
                    var products = await db.Products.Include(p => p.Detail).ToListAsync(stoppingToken);

                    // Mevcut eşleşmeleri çekip hafızaya alalım (Mükerrer kaydı önlemek için)
                    var existingMatches = await db.ProductRuleMatches
                        .Select(m => new { m.ProductId, m.RuleDefinitionId })
                        .ToListAsync(stoppingToken);
                    var existingSet = new HashSet<(int, int)>(existingMatches.Select(x => (x.ProductId, x.RuleDefinitionId)));

                    var matches = new ConcurrentBag<ProductRuleMatch>();

                    // Multithreading burada başlıyor
                    await Parallel.ForEachAsync(products, new ParallelOptions { 
                        MaxDegreeOfParallelism = Environment.ProcessorCount, // Çekirdek sayısı kadar thread
                        CancellationToken = stoppingToken
                    }, (product, token) => {
                        foreach (var rule in rules) {
                            if (IsMatch(product, rule)) {
                                // Eğer daha önce eşleşmediyse listeye ekle
                                if (!existingSet.Contains((product.ProductId, rule.RuleDefinitionId)))
                                {
                                    matches.Add(new ProductRuleMatch { ProductId = product.ProductId, RuleDefinitionId = rule.RuleDefinitionId, MatchedAt = DateTime.Now });
                                }
                            }
                        }
                        return ValueTask.CompletedTask;
                    });

                    if (!matches.IsEmpty)
                    {
                        await db.ProductRuleMatches.AddRangeAsync(matches, stoppingToken);
                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata oluşursa konsola yaz, ama uygulamayı kapatma
                Console.WriteLine($"Worker Hatası: {ex.Message}");
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