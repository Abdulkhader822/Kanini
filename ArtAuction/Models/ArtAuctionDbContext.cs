using Microsoft.EntityFrameworkCore;

namespace ArtAuction.Models
{
    public class ArtAuctionDbContext : DbContext 
    {
        public ArtAuctionDbContext(DbContextOptions<ArtAuctionDbContext> options) : base(options) 
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           

            // User ↔ Artwork (1 : Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Artworks)
                .WithOne(a => a.Artist)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ↔ Bid (1 : Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bids)
                .WithOne(b => b.Buyer)
                .HasForeignKey(b => b.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ↔ Transaction (1 : Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.Buyer)
                .HasForeignKey(t => t.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Artwork ↔ Bid (1 : Many)
            modelBuilder.Entity<Artwork>()
                .HasMany(a => a.Bids)
                .WithOne(b => b.Artwork)
                .HasForeignKey(b => b.ArtworkId);

            // Artwork ↔ Transaction (1 : Many)
            modelBuilder.Entity<Artwork>()
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Artwork)
                .HasForeignKey(t => t.ArtworkId);

            modelBuilder.Entity<Artwork>()
    .Property(a => a.StartingPrice)
    .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Bid>()
                .Property(b => b.BidAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.FinalPrice)
                .HasColumnType("decimal(18,2)");


            // Transaction ↔ Receipt (1 : 1)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Receipt)
                .WithOne(r => r.Transaction)
                .HasForeignKey<Receipt>(r => r.TransactionId);


            // Users
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, FullName = "Admin ", Email = "admin@auction.com", PasswordHash = "admin123", Role = "Admin", CreatedAt = new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                new User { UserId = 2, FullName = "Artist1 ", Email = "artist1@auction.com", PasswordHash = "artist123", Role = "Artist", CreatedAt = new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                new User { UserId = 3, FullName = "Buyer1", Email = "buyer1@auction.com", PasswordHash = "buyer123", Role = "Buyer", CreatedAt = new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
            );

            // Artwork
            modelBuilder.Entity<Artwork>().HasData(
                new Artwork
                {
                    ArtworkId = 1, Title = "Sunset Painting", Description = "Acrylic on canvas",Category = "Painting",
                    ImageData = null,
                    StartingPrice = 1000,
                    AuctionStartTime = new DateTime(2025, 9, 21, 0, 6, 45, 213, DateTimeKind.Local),
                    AuctionEndTime = new DateTime(2025, 9, 26, 0, 6, 45, 214, DateTimeKind.Local),
                    ArtistId = 2 // FK → Artist One
                }
            );

            // Bid
            modelBuilder.Entity<Bid>().HasData(
                new Bid
                {
                    BidId = 1,  BidAmount = 1200,
                    BidTime = new DateTime(2025, 9, 20, 23, 36, 45, 214, DateTimeKind.Local),
                    ArtworkId = 1, // FK → Sunset Painting
                    BuyerId = 3    // FK → Buyer One
                }
            );

            // Transaction (mock completed)
            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    TransactionId = 1, ArtworkId = 1, BuyerId = 3, FinalPrice = 1200,
                    TransactionDate = new DateTime(2025, 9, 21, 0, 6, 45, 215, DateTimeKind.Local),
                    PaymentMethod = "UPI",
                    PaymentStatus = "Completed"

                }
            );

            // Receipt (for that transaction)
            modelBuilder.Entity<Receipt>().HasData(
                new Receipt
                {
                    ReceiptId = 1,  TransactionId = 1, PdfFilePath = "/receipts/txn_1.pdf",
                    GeneratedOn = new DateTime(2025, 9, 21, 0, 6, 45, 215, DateTimeKind.Local)
                }
            );
        }
    }
}

    

