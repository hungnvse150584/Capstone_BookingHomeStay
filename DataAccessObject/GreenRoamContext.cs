using BusinessObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

            /*modelBuilder.Entity<Transaction>()
                .HasOne(e => e.Booking)
            .WithOne(e => e.Transaction)
                .HasForeignKey<Booking>(e => e.transactionID);

            modelBuilder.Entity<Transaction>()
                .HasOne(e => e.BookingService)
            .WithOne(e => e.Transaction)
                .HasForeignKey<BookingServices>(e => e.transactionID);*/

            modelBuilder.Entity<Report>()
                .HasOne(e => e.Booking)
                .WithOne(e => e.Report)
                .HasForeignKey<Booking>(e => e.ReportID);

            modelBuilder.Entity<CancellationPolicy>()
                .HasOne(e => e.HomeStay)
                .WithOne(e => e.CancelPolicy)
                .HasForeignKey<CancellationPolicy>(e => e.HomeStayID);

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
        public DbSet<Pricing> Prices { get; set; }
        public DbSet<CancellationPolicy> CancelPolicy { get; set; }
        public DbSet<CommissionRate> CommissionRates { get; set; }
        public DbSet<CultureExperience> CultureExperiences { get; set; }
        public DbSet<HomeStayRentals> HomeStayRentals { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<RoomTypes> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<BookingServices> BookingServices { get; set; }
        public DbSet<BookingServicesDetail> BookingServicesDetails { get; set; }
        public DbSet<ImageHomeStayRentals> ImageHomeStayRentals { get; set; }
        public DbSet<ImageServices> ImageServices { get; set; }
        public DbSet<ImageRoomTypes> ImageRoomTypes { get; set; }
        public DbSet<ImageCultureExperience> ImageCultureExperiences { get; set; }

        public DbSet<Rating> Rating { set; get; }
        public DbSet<Report> Reports { set; get; }
        public DbSet<Review> Reviews { set; get; }
        public DbSet<RefreshToken> RefreshTokens { set; get; }
        public DbSet<Transaction> Transactions { set; get; }


        //private const string ConnectString = "server=DESKTOP-88329MO\\KHANHVU21;database=GreenRoam;uid=sa;pwd=12345;Integrated Security=true;Trusted_Connection=false;TrustServerCertificate=True";
        //private const string ConnectString = "server=(local);database=GreenRoam;uid=sa;pwd=12345;Integrated Security=true;Trusted_Connection=false;TrustServerCertificate=True";
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(ConnectString);
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer(GetConnectionString());
                }
            }
        }
        private string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                 .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "GreenRoam"))
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();
            var strConn = config["ConnectionStrings:DbConnection"];

            return strConn;
        }
    }
}
