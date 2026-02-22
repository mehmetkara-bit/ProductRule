using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet tanımları
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductDetail> ProductDetails { get; set; }
    public DbSet<RuleDefinition> RuleDefinitions { get; set; }
    public DbSet<ProductRuleMatch> ProductRuleMatches { get; set; }
    public DbSet<ProductTest> ProductTests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Product ↔ ProductDetail (1-1 ilişki)
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.HasOne(e => e.Detail)
                  .WithOne()
                  .HasForeignKey<ProductDetail>(d => d.ProductId);
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.HasKey(e => e.ProductId);
        });

        // RuleDefinition
        modelBuilder.Entity<RuleDefinition>(entity =>
        {
            entity.HasKey(e => e.RuleDefinitionId);

            entity.HasOne(r => r.ProductTest)
                  .WithMany()
                  .HasForeignKey(r => r.ProductTestId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ProductRuleMatch
        modelBuilder.Entity<ProductRuleMatch>(entity =>
        {
            entity.HasKey(e => e.ProductRuleMatchId);

            entity.HasOne(m => m.Product)
                  .WithMany()
                  .HasForeignKey(m => m.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.RuleDefinition)
                  .WithMany()
                  .HasForeignKey(m => m.RuleDefinitionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ProductTest
        modelBuilder.Entity<ProductTest>(entity =>
        {
            entity.HasKey(e => e.ProductTestId);
        });

        base.OnModelCreating(modelBuilder);
    }
}