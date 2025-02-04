using Microsoft.EntityFrameworkCore;
using mobilestore.Models;

namespace mobilestore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductClass> Products { get; set; }
        public DbSet<UserClass> Users { get; set; }
        public DbSet<ItemInCart> CartItems { get; set; }
        public DbSet<ModelOfOrder> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<WishlistClass> Wishlist { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemInCart>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.CartId)
                    .IsRequired();
            });

            modelBuilder.Entity<ModelOfOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.DeliveryAddress)
                    .IsRequired();

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime2");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Quantity)
                    .IsRequired();
            });
        }
    }
} 