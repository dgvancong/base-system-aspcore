using Microsoft.EntityFrameworkCore;
using BaseServerProject.Core.Entities;

namespace BaseServerProject.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        // Tự động log tên database đang kết nối
        var databaseName = Database.GetDbConnection().Database;
        Console.WriteLine($"Connected to database: {databaseName}");
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<SalesOrder> SalesOrders { get; set; }          
    public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
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

        // RefreshToken Configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 👇 THÊM CẤU HÌNH CHO SalesOrder
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

            // Quan hệ 1-n với SalesOrderDetails
            entity.HasMany(e => e.OrderDetails)
                  .WithOne(e => e.Order)
                  .HasForeignKey(e => e.OrderID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 👇 THÊM CẤU HÌNH CHO SalesOrderDetail
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

            // Quan hệ với SalesOrder
            entity.HasOne(e => e.Order)
                  .WithMany(e => e.OrderDetails)
                  .HasForeignKey(e => e.OrderID)
                  .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với Product
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductID)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}