using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JwtID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffIdAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_Staffs_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeStays",
                columns: table => new
                {
                    HomeStayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    CommissionRateID = table.Column<int>(type: "int", nullable: true),
                    CancellationID = table.Column<int>(type: "int", nullable: true),
                    TypeOfRental = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeStays", x => x.HomeStayID);
                    table.ForeignKey(
                        name: "FK_HomeStays_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeStays_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "CancelPolicy",
                columns: table => new
                {
                    CancellationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayBeforeCancel = table.Column<int>(type: "int", nullable: false),
                    RefundPercentage = table.Column<double>(type: "float", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelPolicy", x => x.CancellationID);
                    table.ForeignKey(
                        name: "FK_CancelPolicy_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommissionRates",
                columns: table => new
                {
                    CommissionRateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostShare = table.Column<double>(type: "float", nullable: false),
                    PlatformShare = table.Column<double>(type: "float", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionRates", x => x.CommissionRateID);
                    table.ForeignKey(
                        name: "FK_CommissionRates_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ConversationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User1ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    User2ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ConversationID);
                    table.ForeignKey(
                        name: "FK_Conversations_AspNetUsers_User1ID",
                        column: x => x.User1ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_AspNetUsers_User2ID",
                        column: x => x.User2ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "CultureExperiences",
                columns: table => new
                {
                    CultureExperienceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeStayID = table.Column<int>(type: "int", nullable: true),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CultureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CultureType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CultureExperiences", x => x.CultureExperienceID);
                    table.ForeignKey(
                        name: "FK_CultureExperiences_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CultureExperiences_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "HomeStayRentals",
                columns: table => new
                {
                    HomeStayRentalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    numberBedRoom = table.Column<int>(type: "int", nullable: false),
                    numberBathRoom = table.Column<int>(type: "int", nullable: false),
                    numberKitchen = table.Column<int>(type: "int", nullable: false),
                    numberWifi = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    RentWhole = table.Column<bool>(type: "bit", nullable: false),
                    MaxAdults = table.Column<int>(type: "int", nullable: false),
                    MaxChildren = table.Column<int>(type: "int", nullable: false),
                    MaxPeople = table.Column<int>(type: "int", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeStayRentals", x => x.HomeStayRentalID);
                    table.ForeignKey(
                        name: "FK_HomeStayRentals_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "ImageHomeStays",
                columns: table => new
                {
                    ImageHomeStayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageHomeStays", x => x.ImageHomeStayID);
                    table.ForeignKey(
                        name: "FK_ImageHomeStays_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    RatingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.RatingID);
                    table.ForeignKey(
                        name: "FK_Rating_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rating_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true),
                    BookingID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ServicesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    servicesName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    servicesPrice = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServicesID);
                    table.ForeignKey(
                        name: "FK_Services_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationID = table.Column<int>(type: "int", nullable: false),
                    SenderID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    receiverID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    senderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderID",
                        column: x => x.SenderID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversations",
                        principalColumn: "ConversationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageCultureExperiences",
                columns: table => new
                {
                    ImageCultureExperiencesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CultureExperienceID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageCultureExperiences", x => x.ImageCultureExperiencesID);
                    table.ForeignKey(
                        name: "FK_ImageCultureExperiences_CultureExperiences_CultureExperienceID",
                        column: x => x.CultureExperienceID,
                        principalTable: "CultureExperiences",
                        principalColumn: "CultureExperienceID");
                });

            migrationBuilder.CreateTable(
                name: "ImageHomeStayRentals",
                columns: table => new
                {
                    ImageHomeStayRentalsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeStayRentalID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageHomeStayRentals", x => x.ImageHomeStayRentalsID);
                    table.ForeignKey(
                        name: "FK_ImageHomeStayRentals_HomeStayRentals_HomeStayRentalID",
                        column: x => x.HomeStayRentalID,
                        principalTable: "HomeStayRentals",
                        principalColumn: "HomeStayRentalID");
                });

            migrationBuilder.CreateTable(
                name: "RoomTypes",
                columns: table => new
                {
                    RoomTypesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    numberBedRoom = table.Column<int>(type: "int", nullable: false),
                    numberBathRoom = table.Column<int>(type: "int", nullable: false),
                    numberWifi = table.Column<int>(type: "int", nullable: false),
                    MaxAdults = table.Column<int>(type: "int", nullable: false),
                    MaxChildren = table.Column<int>(type: "int", nullable: false),
                    MaxPeople = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    HomeStayRentalID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypes", x => x.RoomTypesID);
                    table.ForeignKey(
                        name: "FK_RoomTypes_HomeStayRentals_HomeStayRentalID",
                        column: x => x.HomeStayRentalID,
                        principalTable: "HomeStayRentals",
                        principalColumn: "HomeStayRentalID");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    numberOfChildren = table.Column<int>(type: "int", nullable: false),
                    numberOfAdults = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    paymentStatus = table.Column<int>(type: "int", nullable: false),
                    TotalRentPrice = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    bookingDeposit = table.Column<double>(type: "float", nullable: false),
                    remainingBalance = table.Column<double>(type: "float", nullable: false),
                    ReportID = table.Column<int>(type: "int", nullable: true),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingID);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                    table.ForeignKey(
                        name: "FK_Bookings_Reports_ReportID",
                        column: x => x.ReportID,
                        principalTable: "Reports",
                        principalColumn: "ReportID");
                });

            migrationBuilder.CreateTable(
                name: "ImageServices",
                columns: table => new
                {
                    ImageServicesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicesID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageServices", x => x.ImageServicesID);
                    table.ForeignKey(
                        name: "FK_ImageServices_Services_ServicesID",
                        column: x => x.ServicesID,
                        principalTable: "Services",
                        principalColumn: "ServicesID");
                });

            migrationBuilder.CreateTable(
                name: "ImageRoomTypes",
                columns: table => new
                {
                    ImageRoomTypesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomTypesID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageRoomTypes", x => x.ImageRoomTypesID);
                    table.ForeignKey(
                        name: "FK_ImageRoomTypes_RoomTypes_RoomTypesID",
                        column: x => x.RoomTypesID,
                        principalTable: "RoomTypes",
                        principalColumn: "RoomTypesID");
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    PricingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    RentPrice = table.Column<double>(type: "float", nullable: false),
                    Percentage = table.Column<double>(type: "float", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HomeStayRentalID = table.Column<int>(type: "int", nullable: true),
                    RoomTypesID = table.Column<int>(type: "int", nullable: true),
                    DayType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.PricingID);
                    table.ForeignKey(
                        name: "FK_Prices_HomeStayRentals_HomeStayRentalID",
                        column: x => x.HomeStayRentalID,
                        principalTable: "HomeStayRentals",
                        principalColumn: "HomeStayRentalID");
                    table.ForeignKey(
                        name: "FK_Prices_RoomTypes_RoomTypesID",
                        column: x => x.RoomTypesID,
                        principalTable: "RoomTypes",
                        principalColumn: "RoomTypesID");
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roomNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isUsed = table.Column<bool>(type: "bit", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    RoomTypesID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomID);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomTypes_RoomTypesID",
                        column: x => x.RoomTypesID,
                        principalTable: "RoomTypes",
                        principalColumn: "RoomTypesID");
                });

            migrationBuilder.CreateTable(
                name: "BookingServices",
                columns: table => new
                {
                    BookingServicesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingID = table.Column<int>(type: "int", nullable: true),
                    BookingServicesDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    bookingServiceDeposit = table.Column<double>(type: "float", nullable: false),
                    remainingBalance = table.Column<double>(type: "float", nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentServicesMethod = table.Column<int>(type: "int", nullable: false),
                    PaymentServiceStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingServices", x => x.BookingServicesID);
                    table.ForeignKey(
                        name: "FK_BookingServices_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingServices_Bookings_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "BookingID");
                    table.ForeignKey(
                        name: "FK_BookingServices_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateTable(
                name: "BookingDetails",
                columns: table => new
                {
                    BookingDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    rentPrice = table.Column<double>(type: "float", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    HomeStayRentalID = table.Column<int>(type: "int", nullable: true),
                    RoomID = table.Column<int>(type: "int", nullable: true),
                    BookingID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDetails", x => x.BookingDetailID);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Bookings_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "BookingID");
                    table.ForeignKey(
                        name: "FK_BookingDetails_HomeStayRentals_HomeStayRentalID",
                        column: x => x.HomeStayRentalID,
                        principalTable: "HomeStayRentals",
                        principalColumn: "HomeStayRentalID");
                    table.ForeignKey(
                        name: "FK_BookingDetails_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "RoomID");
                });

            migrationBuilder.CreateTable(
                name: "BookingServicesDetails",
                columns: table => new
                {
                    BookingServicesDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<double>(type: "float", nullable: false),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    ServicesID = table.Column<int>(type: "int", nullable: true),
                    BookingServicesID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingServicesDetails", x => x.BookingServicesDetailID);
                    table.ForeignKey(
                        name: "FK_BookingServicesDetails_BookingServices_BookingServicesID",
                        column: x => x.BookingServicesID,
                        principalTable: "BookingServices",
                        principalColumn: "BookingServicesID");
                    table.ForeignKey(
                        name: "FK_BookingServicesDetails_Services_ServicesID",
                        column: x => x.ServicesID,
                        principalTable: "Services",
                        principalColumn: "ServicesID");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingID = table.Column<int>(type: "int", nullable: true),
                    BookingServicesID = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    TypeNotify = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notification_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notification_BookingServices_BookingServicesID",
                        column: x => x.BookingServicesID,
                        principalTable: "BookingServices",
                        principalColumn: "BookingServicesID");
                    table.ForeignKey(
                        name: "FK_Notification_Bookings_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "BookingID");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    ResponseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookingID = table.Column<int>(type: "int", nullable: true),
                    BookingServicesID = table.Column<int>(type: "int", nullable: true),
                    HomeStayID = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TmnCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TxnRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    OrderInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankTranNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecureHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.ResponseId);
                    table.ForeignKey(
                        name: "FK_Transactions_AspNetUsers_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_BookingServices_BookingServicesID",
                        column: x => x.BookingServicesID,
                        principalTable: "BookingServices",
                        principalColumn: "BookingServicesID");
                    table.ForeignKey(
                        name: "FK_Transactions_Bookings_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "BookingID");
                    table.ForeignKey(
                        name: "FK_Transactions_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "269ed675-f6be-4e6a-be52-9622998dabd3", null, "Staff", "STAFF" },
                    { "aba3ace7-0caa-4d5d-a384-fd35eb2dba4a", null, "Customer", "CUSTOMER" },
                    { "c14d5067-3c55-40d4-97a0-ee999ca1e9c0", null, "Admin", "ADMIN" },
                    { "c39df32b-df21-4266-bd0d-37ecc5c077f6", null, "Owner", "OWNER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_BookingID",
                table: "BookingDetails",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_HomeStayRentalID",
                table: "BookingDetails",
                column: "HomeStayRentalID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_RoomID",
                table: "BookingDetails",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AccountID",
                table: "Bookings",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_HomeStayID",
                table: "Bookings",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ReportID",
                table: "Bookings",
                column: "ReportID",
                unique: true,
                filter: "[ReportID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_AccountID",
                table: "BookingServices",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_BookingID",
                table: "BookingServices",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_HomeStayID",
                table: "BookingServices",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingServicesDetails_BookingServicesID",
                table: "BookingServicesDetails",
                column: "BookingServicesID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingServicesDetails_ServicesID",
                table: "BookingServicesDetails",
                column: "ServicesID");

            migrationBuilder.CreateIndex(
                name: "IX_CancelPolicy_HomeStayID",
                table: "CancelPolicy",
                column: "HomeStayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionRates_HomeStayID",
                table: "CommissionRates",
                column: "HomeStayID",
                unique: true,
                filter: "[HomeStayID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_HomeStayID",
                table: "Conversations",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_User1ID",
                table: "Conversations",
                column: "User1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_User2ID",
                table: "Conversations",
                column: "User2ID");

            migrationBuilder.CreateIndex(
                name: "IX_CultureExperiences_AccountID",
                table: "CultureExperiences",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_CultureExperiences_HomeStayID",
                table: "CultureExperiences",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_HomeStayRentals_HomeStayID",
                table: "HomeStayRentals",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_HomeStays_AccountID",
                table: "HomeStays",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_HomeStays_StaffID",
                table: "HomeStays",
                column: "StaffID",
                unique: true,
                filter: "[StaffID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ImageCultureExperiences_CultureExperienceID",
                table: "ImageCultureExperiences",
                column: "CultureExperienceID");

            migrationBuilder.CreateIndex(
                name: "IX_ImageHomeStayRentals_HomeStayRentalID",
                table: "ImageHomeStayRentals",
                column: "HomeStayRentalID");

            migrationBuilder.CreateIndex(
                name: "IX_ImageHomeStays_HomeStayID",
                table: "ImageHomeStays",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_ImageRoomTypes_RoomTypesID",
                table: "ImageRoomTypes",
                column: "RoomTypesID");

            migrationBuilder.CreateIndex(
                name: "IX_ImageServices_ServicesID",
                table: "ImageServices",
                column: "ServicesID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_AccountId",
                table: "Messages",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationID",
                table: "Messages",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderID",
                table: "Messages",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountID",
                table: "Notification",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_BookingID",
                table: "Notification",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_BookingServicesID",
                table: "Notification",
                column: "BookingServicesID");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_HomeStayRentalID",
                table: "Prices",
                column: "HomeStayRentalID");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_RoomTypesID",
                table: "Prices",
                column: "RoomTypesID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_AccountID",
                table: "Rating",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_HomeStayID",
                table: "Rating",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AccountID",
                table: "RefreshTokens",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AccountID",
                table: "Reports",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_HomeStayID",
                table: "Reports",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AccountID",
                table: "Reviews",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_HomeStayID",
                table: "Reviews",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypesID",
                table: "Rooms",
                column: "RoomTypesID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_HomeStayRentalID",
                table: "RoomTypes",
                column: "HomeStayRentalID");

            migrationBuilder.CreateIndex(
                name: "IX_Services_HomeStayID",
                table: "Services",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_AccountID",
                table: "Staffs",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BookingID",
                table: "Transactions",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BookingServicesID",
                table: "Transactions",
                column: "BookingServicesID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_HomeStayID",
                table: "Transactions",
                column: "HomeStayID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BookingDetails");

            migrationBuilder.DropTable(
                name: "BookingServicesDetails");

            migrationBuilder.DropTable(
                name: "CancelPolicy");

            migrationBuilder.DropTable(
                name: "CommissionRates");

            migrationBuilder.DropTable(
                name: "ImageCultureExperiences");

            migrationBuilder.DropTable(
                name: "ImageHomeStayRentals");

            migrationBuilder.DropTable(
                name: "ImageHomeStays");

            migrationBuilder.DropTable(
                name: "ImageRoomTypes");

            migrationBuilder.DropTable(
                name: "ImageServices");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "CultureExperiences");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "BookingServices");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "HomeStayRentals");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "HomeStays");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
