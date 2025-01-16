using BusinessObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class GreenRoamContext : IdentityDbContext<Account>
    {
        public GreenRoamContext() : base()
        { }
        public GreenRoamContext(DbContextOptions<GreenRoamContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne(e => e.Booking)
            .WithOne(e => e.Transaction)
                .HasForeignKey<Booking>(e => e.transactionID);

            modelBuilder.Entity<Transaction>()
                .HasOne(e => e.BookingService)
            .WithOne(e => e.Transaction)
                .HasForeignKey<BookingServices>(e => e.transactionID);

            modelBuilder.Entity<Report>()
                .HasOne(e => e.Booking)
                .WithOne(e => e.Report)
                .HasForeignKey<Booking>(e => e.ReportID);

            List<IdentityRole> roles = new List<IdentityRole>
              {
                  new IdentityRole
                  {
                      Name = "Admin",
                      NormalizedName = "ADMIN"
                  },
                  new IdentityRole
                  {
                      Name = "Customer",
                      NormalizedName = "CUSTOMER"
                  },
                  new IdentityRole
                 {
                      Name = "Owner",
                      NormalizedName = "OWNER"
                  }
             };
            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
        public DbSet<HomeStay> HomeStays { get; set; }
        public DbSet<HomeStayTypes> HomeStayTypes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<BookingServices> BookingServices { get; set; }
        public DbSet<BookingServicesDetail> BookingServicesDetails { get; set; }
        public DbSet<ImageHomeStayTypes> ImageHomeStayTypes { get; set; }
        public DbSet<ImageServices> ImageServices { get; set; }

        public DbSet<Rating> Rating { set; get; }
        public DbSet<Report> Reports { set; get; }
        public DbSet<Review> Reviews { set; get; }
        public DbSet<RefreshToken> RefreshTokens { set; get; }
        public DbSet<Transaction> Transactions { set; get; }

        public DbSet<Location> Locations { set; get; }
        public DbSet<Province> Provinces { set; get; }
        public DbSet<District> Districts { set; get; }
        public DbSet<Ward> Wards { set; get; }
        public DbSet<Street> Streets { set; get; }


        private const string ConnectString = "server=(local);database=GreenRoam;uid=sa;pwd=12345;Integrated Security=true;Trusted_Connection=false;TrustServerCertificate=True";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectString);
        }
    }
}
