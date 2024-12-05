using Microsoft.EntityFrameworkCore;
using NextStopEndPoints.Models;

namespace NextStopEndPoints.Data
{
    public class NextStopDbContext : DbContext
    {
        public NextStopDbContext() { }
        public NextStopDbContext(DbContextOptions<NextStopDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Seat to Booking as optional
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Booking)
                .WithMany(b => b.Seats)
                .HasForeignKey(s => s.BookingId)
                .IsRequired(false);

            // Unique constraint on BusNumber
            modelBuilder.Entity<Bus>()
                .HasIndex(b => b.BusNumber)
                .IsUnique();

            // Make Email property unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Set precision for decimal properties
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalFare)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Models.Route>()
                .Property(r => r.Distance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Schedule>()
                .Property(s => s.Fare)
                .HasColumnType("decimal(18,2)");

            // Configure foreign keys with restricted cascading behavior
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascading on delete

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade); // Keep cascading only where needed
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AdminAction> AdminActions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


    }
}
