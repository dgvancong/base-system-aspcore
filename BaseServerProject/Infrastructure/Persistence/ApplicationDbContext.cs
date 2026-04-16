using Microsoft.EntityFrameworkCore;
using BaseServerProject.Core.Entities;

namespace BaseServerProject.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        var databaseName = Database.GetDbConnection().Database;
        Console.WriteLine($"Connected to database: {databaseName}");
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<SalesOrder> SalesOrders { get; set; }
    public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========== CẤU HÌNH CHO PRODUCT ==========
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID);
            entity.Property(e => e.ProductID).UseIdentityColumn();

            entity.Property(e => e.ProductCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            // Quan hệ với ProductVariant - sử dụng đúng tên property
            entity.HasMany(p => p.Variants)
                  .WithOne(v => v.Product)
                  .HasForeignKey(v => v.ProductID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ========== CẤU HÌNH CHO PRODUCT VARIANT ==========
        modelBuilder.Entity<ProductVariant>(entity =>
        {
           

            entity.Property(e => e.SKU)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PurchasePrice)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.SellingPrice)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.QuantityInStock)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.Status)  
                .HasMaxLength(20)
                .HasDefaultValue("Đang bán");

            entity.HasOne(v => v.Product)
                  .WithMany(p => p.Variants)
                  .HasForeignKey(v => v.ProductID)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(v => v.Color)
                  .WithMany()
                  .HasForeignKey(v => v.ColorID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Size)
                  .WithMany()
                  .HasForeignKey(v => v.SizeID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductVariant>()
            .HasOne(v => v.Color)
            .WithMany(c => c.ProductVariants)
            .HasForeignKey(v => v.ColorID)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ProductVariant>()
            .HasOne(v => v.Size)
            .WithMany(s => s.ProductVariants) 
            .HasForeignKey(v => v.SizeID)
            .OnDelete(DeleteBehavior.Restrict);


        // ========== CẤU HÌNH CHO USER ==========
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .IsRequired();

            entity.Property(e => e.FirstName)
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .HasMaxLength(100);

            entity.Property(e => e.Status)
                .HasConversion<int>();
        });

        // ========== CẤU HÌNH CHO REFRESH TOKEN ==========
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========== CẤU HÌNH CHO SALES ORDER ==========
        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.HasKey(e => e.OrderID);
            entity.Property(e => e.OrderID).UseIdentityColumn();

            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasDefaultValue("Tiền mặt");

            entity.Property(e => e.OrderStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Hoàn thành");

            entity.Property(e => e.SalesPerson)
                .HasMaxLength(100);

            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);

            entity.HasMany(e => e.OrderDetails)
                  .WithOne(e => e.Order)
                  .HasForeignKey(e => e.OrderID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ========== CẤU HÌNH CHO SALES ORDER DETAIL ==========
        modelBuilder.Entity<SalesOrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailID);
            entity.Property(e => e.OrderDetailID).UseIdentityColumn();

            entity.Property(e => e.ProductCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            entity.HasOne(e => e.Order)
                  .WithMany(e => e.OrderDetails)
                  .HasForeignKey(e => e.OrderID)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductID)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}