using Microsoft.EntityFrameworkCore;
using Pet_Shop.Models.Entities;

namespace Pet_Shop.Data
{
    public class PetShopDbContext : DbContext
    {
        public PetShopDbContext(DbContextOptions<PetShopDbContext> options) : base(options)
        {
        }

        // User Management
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }

        // Product Management
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        // Address Management
        public DbSet<Address> Addresses { get; set; }

        // Cart Management
        public DbSet<Cart> Cart { get; set; }

        // Promotion Management
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<ProductPromotion> ProductPromotions { get; set; }

        // Order Management
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }

        // Banner Management
        public DbSet<Banner> Banners { get; set; }

        // Contact Management
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserRole
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.RoleID);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.RoleID);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CustomerProfile
            modelBuilder.Entity<CustomerProfile>(entity =>
            {
                entity.HasKey(e => e.ProfileID);
                entity.Property(e => e.TotalSpent).HasColumnType("decimal(15,2)");
                entity.Property(e => e.MembershipLevel).HasMaxLength(20).HasDefaultValue("Bronze");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.CustomerProfile)
                    .HasForeignKey<CustomerProfile>(d => d.UserID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryID);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);

                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.SubCategories)
                    .HasForeignKey(d => d.ParentCategoryID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Brand
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.BrandID);
                entity.Property(e => e.BrandName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.BrandName).IsUnique();
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductID);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ProductCode).HasMaxLength(50);
                entity.Property(e => e.ProductType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PetType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Weight).HasColumnType("decimal(8,2)");
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(15,2)");
                entity.Property(e => e.SalePrice).HasColumnType("decimal(15,2)");
                entity.Property(e => e.Cost).HasColumnType("decimal(15,2)");
                entity.HasIndex(e => e.ProductCode).IsUnique();
                entity.HasIndex(e => e.CategoryID);
                entity.HasIndex(e => e.BrandID);
                entity.HasIndex(e => e.ProductType);
                entity.HasIndex(e => e.PetType);
                entity.HasIndex(e => e.IsActive);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ProductImage
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.ImageID);
                entity.Property(e => e.ImageURL).IsRequired().HasMaxLength(255);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Inventory
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.InventoryID);
                entity.HasIndex(e => e.ProductID);

                entity.HasOne(d => d.Product)
                    .WithOne(p => p.Inventory)
                    .HasForeignKey<Inventory>(d => d.ProductID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure InventoryTransaction
            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionID);
                entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(15,2)");
                entity.Property(e => e.TotalValue).HasColumnType("decimal(15,2)");
                entity.Property(e => e.ReferenceNumber).HasMaxLength(50);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.InventoryTransactions)
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.InventoryTransactions)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Address
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.AddressID);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.AddressLine).IsRequired().HasMaxLength(200);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Cart
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.CartID);
                entity.HasIndex(e => new { e.UserID, e.ProductID }).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Promotion
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.HasKey(e => e.PromotionID);
                entity.Property(e => e.PromotionCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PromotionName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DiscountType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DiscountValue).IsRequired().HasColumnType("decimal(15,2)");
                entity.Property(e => e.MinOrderAmount).HasColumnType("decimal(15,2)");
                entity.Property(e => e.MaxDiscountAmount).HasColumnType("decimal(15,2)");
                entity.HasIndex(e => e.PromotionCode).IsUnique();
            });

            // Configure ProductPromotion
            modelBuilder.Entity<ProductPromotion>(entity =>
            {
                entity.HasKey(e => e.ProductPromotionID);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPromotions)
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.ProductPromotions)
                    .HasForeignKey(d => d.PromotionID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderStatus
            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasKey(e => e.StatusID);
                entity.Property(e => e.StatusName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.StatusName).IsUnique();
                
                // Configure relationship with Orders
                entity.HasMany(d => d.Orders)
                    .WithOne(p => p.Status)
                    .HasForeignKey(d => d.StatusID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PaymentMethod
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.PaymentMethodID);
                entity.Property(e => e.MethodName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.MethodName).IsUnique();
            });

            // Configure Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderID);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SubTotal).IsRequired().HasColumnType("decimal(15,2)");
                entity.Property(e => e.ShippingFee).HasColumnType("decimal(15,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(15,2)");
                entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(15,2)");
                entity.Property(e => e.ShippingAddress).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.UserID);
                entity.HasIndex(e => e.StatusID);
                entity.HasIndex(e => e.OrderDate);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StatusID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentMethodID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PromotionID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemID);
                entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(15,2)");
                entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(15,2)");
                entity.HasIndex(e => e.OrderID);
                entity.HasIndex(e => e.ProductID);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OrderStatusHistory
            modelBuilder.Entity<OrderStatusHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryID);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderStatusHistories)
                    .HasForeignKey(d => d.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.OldStatus)
                    .WithMany()
                    .HasForeignKey(d => d.OldStatusID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.NewStatus)
                    .WithMany()
                    .HasForeignKey(d => d.NewStatusID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ChangedByUser)
                    .WithMany(p => p.OrderStatusHistories)
                    .HasForeignKey(d => d.ChangedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Banner
            modelBuilder.Entity<Banner>(entity =>
            {
                entity.HasKey(e => e.BannerID);
                entity.Property(e => e.BannerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ImageURL).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Position).HasMaxLength(50);
            });

            // Configure ContactMessage
            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.HasKey(e => e.MessageID);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("New");

                entity.HasOne(d => d.RepliedByUser)
                    .WithMany(p => p.ContactMessages)
                    .HasForeignKey(d => d.RepliedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
