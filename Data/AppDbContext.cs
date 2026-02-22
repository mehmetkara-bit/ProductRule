using Microsoft.EntityFrameworkCore;

namespace ProductRule.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<RuleDefinition> Rules { get; set; }
        public DbSet<ProductRuleMatch> ProductRuleMatches { get; set; }
        public DbSet<ProductTest> ProductTests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductNo);
                entity.HasOne(e => e.Detail)
                    .WithOne()
                    .HasForeignKey<ProductDetail>(e => e.ProductNo);
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.HasKey(e => e.ProductNo);
            });

            modelBuilder.Entity<RuleDefinition>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ProductRuleMatch>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ProductTest>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
