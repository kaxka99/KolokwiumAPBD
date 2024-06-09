using KolokwiumCF.Model;
using Microsoft.EntityFrameworkCore;

namespace KolokwiumCF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<Payment>().ToTable("Payment");
            modelBuilder.Entity<Discount>().ToTable("Discount");
            modelBuilder.Entity<Sale>().ToTable("Sale");
            // Konfiguracja dla tabeli Client
            modelBuilder.Entity<Client>()
                .HasKey(c => c.IdClient);

            // Konfiguracja dla tabeli Subscription
            modelBuilder.Entity<Subscription>()
                .HasKey(s => s.IdSubscription);

            // Konfiguracja dla tabeli Payment
            modelBuilder.Entity<Payment>()
                .HasKey(p => p.IdPayment);
            modelBuilder.Entity<Payment>()
                .Property(p => p.Value)
                .HasColumnType("decimal(18, 2)");

            // Konfiguracja dla tabeli Discount
            modelBuilder.Entity<Discount>()
                .HasKey(d => d.IdDiscount);

            // Konfiguracja dla tabeli Sale
            modelBuilder.Entity<Sale>()
                .HasKey(s => s.IdSale);

            // Relacje
            modelBuilder.Entity<Discount>()
                .HasOne<Client>()
                .WithMany()
                .HasForeignKey(d => d.IdClient);

            modelBuilder.Entity<Payment>()
                .HasOne<Client>()
                .WithMany()
                .HasForeignKey(p => p.IdClient);

            modelBuilder.Entity<Payment>()
                .HasOne<Subscription>()
                .WithMany()
                .HasForeignKey(p => p.IdSubscription);

            modelBuilder.Entity<Sale>()
                .HasOne<Client>()
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.IdClient);

            modelBuilder.Entity<Sale>()
                .HasOne<Subscription>()
                .WithMany()
                .HasForeignKey(s => s.IdSubscription);
        }
    }

}
