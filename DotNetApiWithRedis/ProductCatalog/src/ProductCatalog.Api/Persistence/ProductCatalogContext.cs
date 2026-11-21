using Microsoft.EntityFrameworkCore;
using System;

internal class ProductCatalogContext(DbContextOptions<ProductCatalogContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });
    }

    public sealed class Product
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
    }

    public sealed class ProductResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
    }

    public sealed class CreateProductRequest
    {
        public required string Name { get; set; }
        public required decimal Price { get; set; }
    }
}