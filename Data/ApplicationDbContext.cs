using Microsoft.EntityFrameworkCore;
using MyntraClone.API.Models;

namespace MyntraClone.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- DbSets: define one table per entity ---
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ─── USER RELATIONSHIPS ─────────────────────────────────

            // 1:1: User → Cart
            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)                     // Each User has one Cart
                .WithOne(c => c.User)                    // Each Cart has one User
                .HasForeignKey<Cart>(c => c.UserId);     // FK in Cart.UserId

            // 1:1: User → Wishlist
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wishlist)                 // Each User has one Wishlist
                .WithOne(w => w.User)                    // Each Wishlist has one User
                .HasForeignKey<Wishlist>(w => w.UserId); // FK in Wishlist.UserId

            // 1:M: User → Orders
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)                  // A User can have many Orders
                .WithOne(o => o.User)                    // Each Order belongs to one User
                .HasForeignKey(o => o.UserId);           // FK in Order.UserId

            // ─── CATEGORY RELATIONSHIPS ──────────────────────────────

            // 1:M: Category → Products
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)                // A Category can have many Products
                .WithOne(p => p.Category)                // Each Product belongs to one Category
                .HasForeignKey(p => p.CategoryId);       // FK in Product.CategoryId

            // ─── CART RELATIONSHIPS ──────────────────────────────────

            // 1:M: Cart → CartItems
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)               // A Cart holds many CartItems
                .WithOne(ci => ci.Cart)                  // Each CartItem belongs to one Cart
                .HasForeignKey(ci => ci.CartId);         // FK in CartItem.CartId

            // M:1: CartItem → Product
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)                // Each CartItem refers to one Product
                .WithMany(p => p.CartItems)              // A Product can appear in many CartItems
                .HasForeignKey(ci => ci.ProductId);      // FK in CartItem.ProductId

            // ─── WISHLIST RELATIONSHIPS ─────────────────────────────

            // 1:M: Wishlist → WishlistItems
            modelBuilder.Entity<Wishlist>()
                .HasMany(w => w.WishlistItems)           // A Wishlist holds many WishlistItems
                .WithOne(wi => wi.Wishlist)              // Each WishlistItem belongs to one Wishlist
                .HasForeignKey(wi => wi.WishlistId);     // FK in WishlistItem.WishlistId

            // M:1: WishlistItem → Product
            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Product)                // Each WishlistItem refers to one Product
                .WithMany(p => p.WishlistItems)          // A Product can appear in many WishlistItems
                .HasForeignKey(wi => wi.ProductId);      // FK in WishlistItem.ProductId

            // ─── ORDER RELATIONSHIPS ────────────────────────────────

            // 1:M: Order → OrderItems
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)              // An Order contains many OrderItems
                .WithOne(oi => oi.Order)                 // Each OrderItem belongs to one Order
                .HasForeignKey(oi => oi.OrderId);        // FK in OrderItem.OrderId

            // M:1: OrderItem → Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)                // Each OrderItem refers to one Product
                .WithMany(p => p.OrderItems)             // A Product can appear in many OrderItems
                .HasForeignKey(oi => oi.ProductId);      // FK in OrderItem.ProductId

            // ─── DECIMAL PRECISION CONFIGURATION ────────────────────

            // Ensure money fields have 18,2 precision to avoid truncation
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountPrice)
                .HasColumnType("decimal(18,2)");
        }
    }
}
