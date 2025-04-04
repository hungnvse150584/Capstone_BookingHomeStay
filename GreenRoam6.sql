CREATE DATABASE [GreenRoam]
GO
USE [GreenRoam]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Address] [nvarchar](max) NOT NULL,
	[Phone] [nvarchar](max) NOT NULL,
	[TaxCode] [nvarchar](max) NULL,
	[BankAccountNumber] [nvarchar](max) NOT NULL,
	[Status] [bit] NOT NULL,
	[DateOfBirth] [datetime2](7) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingDetails]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingDetails](
	[BookingDetailID] [int] IDENTITY(1,1) NOT NULL,
	[rentPrice] [float] NOT NULL,
	[CheckInDate] [datetime2](7) NOT NULL,
	[CheckOutDate] [datetime2](7) NOT NULL,
	[TotalAmount] [float] NOT NULL,
	[HomeStayRentalID] [int] NULL,
	[RoomID] [int] NULL,
	[BookingID] [int] NULL,
	[UnitPrice] [float] NOT NULL,
 CONSTRAINT [PK_BookingDetails] PRIMARY KEY CLUSTERED 
(
	[BookingDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[BookingID] [int] IDENTITY(1,1) NOT NULL,
	[BookingDate] [datetime2](7) NOT NULL,
	[ExpiredTime] [datetime2](7) NOT NULL,
	[numberOfChildren] [int] NOT NULL,
	[numberOfAdults] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[paymentStatus] [int] NOT NULL,
	[Total] [float] NOT NULL,
	[bookingDeposit] [float] NOT NULL,
	[ReportID] [int] NULL,
	[AccountID] [nvarchar](450) NOT NULL,
	[HomeStayID] [int] NULL,
	[PaymentMethod] [int] NOT NULL,
	[remainingBalance] [float] NOT NULL,
 CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED 
(
	[BookingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingServices]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingServices](
	[BookingServicesID] [int] IDENTITY(1,1) NOT NULL,
	[BookingID] [int] NULL,
	[BookingServicesDate] [datetime2](7) NOT NULL,
	[Total] [float] NOT NULL,
	[AccountID] [nvarchar](450) NOT NULL,
	[Status] [int] NOT NULL,
	[PaymentServicesMethod] [int] NOT NULL,
	[bookingServiceDeposit] [float] NOT NULL,
	[remainingBalance] [float] NOT NULL,
 CONSTRAINT [PK_BookingServices] PRIMARY KEY CLUSTERED 
(
	[BookingServicesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingServicesDetails]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingServicesDetails](
	[BookingServicesDetailID] [int] IDENTITY(1,1) NOT NULL,
	[Quantity] [int] NOT NULL,
	[unitPrice] [float] NOT NULL,
	[TotalAmount] [float] NOT NULL,
	[ServicesID] [int] NULL,
	[BookingServicesID] [int] NULL,
 CONSTRAINT [PK_BookingServicesDetails] PRIMARY KEY CLUSTERED 
(
	[BookingServicesDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CancelPolicy]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CancelPolicy](
	[CancellationID] [int] IDENTITY(1,1) NOT NULL,
	[DayBeforeCancel] [int] NOT NULL,
	[RefundPercentage] [float] NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[HomeStayID] [int] NOT NULL,
 CONSTRAINT [PK_CancelPolicy] PRIMARY KEY CLUSTERED 
(
	[CancellationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CommissionRates]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommissionRates](
	[CommissionRateID] [int] IDENTITY(1,1) NOT NULL,
	[HostShare] [float] NOT NULL,
	[PlatformShare] [float] NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_CommissionRates] PRIMARY KEY CLUSTERED 
(
	[CommissionRateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CultureExperiences]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CultureExperiences](
	[CultureExperienceID] [int] IDENTITY(1,1) NOT NULL,
	[HomeStayID] [int] NULL,
	[AccountID] [nvarchar](450) NOT NULL,
	[CultureName] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[CultureType] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_CultureExperiences] PRIMARY KEY CLUSTERED 
(
	[CultureExperienceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HomeStayRentals]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HomeStayRentals](
	[HomeStayRentalID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[numberBedRoom] [int] NOT NULL,
	[numberBathRoom] [int] NOT NULL,
	[numberKitchen] [int] NOT NULL,
	[numberWifi] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[RentWhole] [bit] NOT NULL,
	[MaxAdults] [int] NOT NULL,
	[MaxChildren] [int] NOT NULL,
	[MaxPeople] [int] NOT NULL,
	[HomeStayID] [int] NULL,
 CONSTRAINT [PK_HomeStayRentals] PRIMARY KEY CLUSTERED 
(
	[HomeStayRentalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HomeStays]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HomeStays](
	[HomeStayID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Address] [nvarchar](max) NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[Status] [int] NOT NULL,
	[Area] [nvarchar](max) NOT NULL,
	[AccountID] [nvarchar](450) NOT NULL,
	[CommissionRateID] [int] NULL,
	[TypeOfRental] [int] NOT NULL,
	[CancellationID] [int] NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_HomeStays] PRIMARY KEY CLUSTERED 
(
	[HomeStayID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImageCultureExperiences]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImageCultureExperiences](
	[ImageCultureExperiencesID] [int] IDENTITY(1,1) NOT NULL,
	[Image] [nvarchar](max) NOT NULL,
	[CultureExperienceID] [int] NULL,
 CONSTRAINT [PK_ImageCultureExperiences] PRIMARY KEY CLUSTERED 
(
	[ImageCultureExperiencesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImageHomeStayRentals]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImageHomeStayRentals](
	[ImageHomeStayRentalsID] [int] IDENTITY(1,1) NOT NULL,
	[Image] [nvarchar](max) NOT NULL,
	[HomeStayRentalID] [int] NULL,
 CONSTRAINT [PK_ImageHomeStayRentals] PRIMARY KEY CLUSTERED 
(
	[ImageHomeStayRentalsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImageRoomTypes]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImageRoomTypes](
	[ImageRoomTypesID] [int] IDENTITY(1,1) NOT NULL,
	[Image] [nvarchar](max) NOT NULL,
	[RoomTypesID] [int] NULL,
 CONSTRAINT [PK_ImageRoomTypes] PRIMARY KEY CLUSTERED 
(
	[ImageRoomTypesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImageServices]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImageServices](
	[ImageServicesID] [int] IDENTITY(1,1) NOT NULL,
	[Image] [nvarchar](max) NOT NULL,
	[ServicesID] [int] NULL,
 CONSTRAINT [PK_ImageServices] PRIMARY KEY CLUSTERED 
(
	[ImageServicesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[NotificationID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [nvarchar](450) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[BookingID] [int] NULL,
	[BookingServicesID] [int] NULL,
	[IsRead] [bit] NOT NULL,
	[TypeNotify] [int] NOT NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[NotificationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Prices]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prices](
	[PricingID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[UnitPrice] [float] NOT NULL,
	[RentPrice] [float] NOT NULL,
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[IsDefault] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[HomeStayRentalID] [int] NULL,
	[RoomTypesID] [int] NULL,
 CONSTRAINT [PK_Prices] PRIMARY KEY CLUSTERED 
(
	[PricingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rating]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rating](
	[RatingID] [int] IDENTITY(1,1) NOT NULL,
	[Rate] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[AccountID] [nvarchar](450) NOT NULL,
	[HomeStayID] [int] NULL,
 CONSTRAINT [PK_Rating] PRIMARY KEY CLUSTERED 
(
	[RatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefreshTokens]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshTokens](
	[Id] [uniqueidentifier] NOT NULL,
	[AccountID] [nvarchar](450) NOT NULL,
	[Token] [nvarchar](max) NOT NULL,
	[JwtID] [nvarchar](max) NOT NULL,
	[IsUsed] [bit] NOT NULL,
	[IsRevoked] [bit] NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[ExpiredAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reports]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reports](
	[ReportID] [int] IDENTITY(1,1) NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[ReportText] [nvarchar](max) NOT NULL,
	[ResponseText] [nvarchar](max) NOT NULL,
	[Status] [bit] NOT NULL,
	[Image] [nvarchar](max) NULL,
	[AccountID] [nvarchar](450) NOT NULL,
	[HomeStayID] [int] NULL,
	[BookingID] [int] NULL,
 CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reviews]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[ReviewID] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[AccountID] [nvarchar](450) NOT NULL,
	[HomeStayID] [int] NULL,
 CONSTRAINT [PK_Reviews] PRIMARY KEY CLUSTERED 
(
	[ReviewID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rooms]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rooms](
	[RoomID] [int] IDENTITY(1,1) NOT NULL,
	[roomNumber] [nvarchar](max) NOT NULL,
	[isUsed] [bit] NOT NULL,
	[RoomTypesID] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_Rooms] PRIMARY KEY CLUSTERED 
(
	[RoomID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoomTypes]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomTypes](
	[RoomTypesID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[numberBedRoom] [int] NOT NULL,
	[numberBathRoom] [int] NOT NULL,
	[numberWifi] [int] NOT NULL,
	[MaxAdults] [int] NOT NULL,
	[MaxChildren] [int] NOT NULL,
	[MaxPeople] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[HomeStayRentalID] [int] NULL,
 CONSTRAINT [PK_RoomTypes] PRIMARY KEY CLUSTERED 
(
	[RoomTypesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Services]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Services](
	[ServicesID] [int] IDENTITY(1,1) NOT NULL,
	[servicesName] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[UnitPrice] [float] NOT NULL,
	[servicesPrice] [float] NOT NULL,
	[Status] [bit] NOT NULL,
	[HomeStayID] [int] NULL,
 CONSTRAINT [PK_Services] PRIMARY KEY CLUSTERED 
(
	[ServicesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 3/17/2025 9:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[ResponseId] [nvarchar](450) NOT NULL,
	[TmnCode] [nvarchar](max) NOT NULL,
	[TxnRef] [nvarchar](max) NOT NULL,
	[Amount] [bigint] NOT NULL,
	[OrderInfo] [nvarchar](max) NOT NULL,
	[ResponseCode] [nvarchar](max) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[BankTranNo] [nvarchar](max) NOT NULL,
	[PayDate] [datetime2](7) NOT NULL,
	[BankCode] [nvarchar](max) NOT NULL,
	[TransactionNo] [nvarchar](max) NOT NULL,
	[TransactionType] [nvarchar](max) NOT NULL,
	[TransactionStatus] [nvarchar](max) NOT NULL,
	[SecureHash] [nvarchar](max) NOT NULL,
	[BookingID] [int] NULL,
	[BookingServicesID] [int] NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[ResponseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[BookingDetails] ADD  DEFAULT ((0.0000000000000000e+000)) FOR [UnitPrice]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ((0.0000000000000000e+000)) FOR [remainingBalance]
GO
ALTER TABLE [dbo].[BookingServices] ADD  DEFAULT ((0.0000000000000000e+000)) FOR [bookingServiceDeposit]
GO
ALTER TABLE [dbo].[BookingServices] ADD  DEFAULT ((0.0000000000000000e+000)) FOR [remainingBalance]
GO
ALTER TABLE [dbo].[HomeStays] ADD  DEFAULT ((0.0000000000000000e+000)) FOR [Latitude]
GO
ALTER TABLE [dbo].[HomeStays] ADD  DEFAULT ((0.0000000000000000e+000)) FOR [Longitude]
GO
ALTER TABLE [dbo].[Rooms] ADD  DEFAULT (CONVERT([bit],(0))) FOR [isActive]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[BookingDetails]  WITH CHECK ADD  CONSTRAINT [FK_BookingDetails_Bookings_BookingID] FOREIGN KEY([BookingID])
REFERENCES [dbo].[Bookings] ([BookingID])
GO
ALTER TABLE [dbo].[BookingDetails] CHECK CONSTRAINT [FK_BookingDetails_Bookings_BookingID]
GO
ALTER TABLE [dbo].[BookingDetails]  WITH CHECK ADD  CONSTRAINT [FK_BookingDetails_HomeStayRentals_HomeStayRentalID] FOREIGN KEY([HomeStayRentalID])
REFERENCES [dbo].[HomeStayRentals] ([HomeStayRentalID])
GO
ALTER TABLE [dbo].[BookingDetails] CHECK CONSTRAINT [FK_BookingDetails_HomeStayRentals_HomeStayRentalID]
GO
ALTER TABLE [dbo].[BookingDetails]  WITH CHECK ADD  CONSTRAINT [FK_BookingDetails_Rooms_RoomID] FOREIGN KEY([RoomID])
REFERENCES [dbo].[Rooms] ([RoomID])
GO
ALTER TABLE [dbo].[BookingDetails] CHECK CONSTRAINT [FK_BookingDetails_Rooms_RoomID]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_AspNetUsers_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_AspNetUsers_AccountID]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_HomeStays_HomeStayID] FOREIGN KEY([HomeStayID])
REFERENCES [dbo].[HomeStays] ([HomeStayID])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_HomeStays_HomeStayID]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_Reports_ReportID] FOREIGN KEY([ReportID])
REFERENCES [dbo].[Reports] ([ReportID])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_Reports_ReportID]
GO
ALTER TABLE [dbo].[BookingServices]  WITH CHECK ADD  CONSTRAINT [FK_BookingServices_AspNetUsers_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BookingServices] CHECK CONSTRAINT [FK_BookingServices_AspNetUsers_AccountID]
GO
ALTER TABLE [dbo].[BookingServices]  WITH CHECK ADD  CONSTRAINT [FK_BookingServices_Bookings_BookingID] FOREIGN KEY([BookingID])
REFERENCES [dbo].[Bookings] ([BookingID])
GO
ALTER TABLE [dbo].[BookingServices] CHECK CONSTRAINT [FK_BookingServices_Bookings_BookingID]
GO
ALTER TABLE [dbo].[BookingServicesDetails]  WITH CHECK ADD  CONSTRAINT [FK_BookingServicesDetails_BookingServices_BookingServicesID] FOREIGN KEY([BookingServicesID])
REFERENCES [dbo].[BookingServices] ([BookingServicesID])
GO
ALTER TABLE [dbo].[BookingServicesDetails] CHECK CONSTRAINT [FK_BookingServicesDetails_BookingServices_BookingServicesID]
GO
ALTER TABLE [dbo].[BookingServicesDetails]  WITH CHECK ADD  CONSTRAINT [FK_BookingServicesDetails_Services_ServicesID] FOREIGN KEY([ServicesID])
REFERENCES [dbo].[Services] ([ServicesID])
GO
ALTER TABLE [dbo].[BookingServicesDetails] CHECK CONSTRAINT [FK_BookingServicesDetails_Services_ServicesID]
GO
ALTER TABLE [dbo].[CancelPolicy]  WITH CHECK ADD  CONSTRAINT [FK_CancelPolicy_HomeStays_HomeStayID] FOREIGN KEY([HomeStayID])
REFERENCES [dbo].[HomeStays] ([HomeStayID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CancelPolicy] CHECK CONSTRAINT [FK_CancelPolicy_HomeStays_HomeStayID]
GO
ALTER TABLE [dbo].[CultureExperiences]  WITH CHECK ADD  CONSTRAINT [FK_CultureExperiences_AspNetUsers_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CultureExperiences] CHECK CONSTRAINT [FK_CultureExperiences_AspNetUsers_AccountID]
GO
ALTER TABLE [dbo].[CultureExperiences]  WITH CHECK ADD  CONSTRAINT [FK_CultureExperiences_HomeStays_HomeStayID] FOREIGN KEY([HomeStayID])
REFERENCES [dbo].[HomeStays] ([HomeStayID])
GO
ALTER TABLE [dbo].[CultureExperiences] CHECK CONSTRAINT [FK_CultureExperiences_HomeStays_HomeStayID]
GO
ALTER TABLE [dbo].[HomeStayRentals]  WITH CHECK ADD  CONSTRAINT [FK_HomeStayRentals_HomeStays_HomeStayID] FOREIGN KEY([HomeStayID])
REFERENCES [dbo].[HomeStays] ([HomeStayID])
GO
ALTER TABLE [dbo].[HomeStayRentals] CHECK CONSTRAINT [FK_HomeStayRentals_HomeStays_HomeStayID]
GO
ALTER TABLE [dbo].[HomeStays]  WITH CHECK ADD  CONSTRAINT [FK_HomeStays_AspNetUsers_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HomeStays] CHECK CONSTRAINT [FK_HomeStays_AspNetUsers_AccountID]
GO
ALTER TABLE [dbo].[HomeStays]  WITH CHECK ADD  CONSTRAINT [FK_HomeStays_CommissionRates_CommissionRateID] FOREIGN KEY([CommissionRateID])
REFERENCES [dbo].[CommissionRates] ([CommissionRateID])
GO
ALTER TABLE [dbo].[HomeStays] CHECK CONSTRAINT [FK_HomeStays_CommissionRates_CommissionRateID]
GO
ALTER TABLE [dbo].[ImageCultureExperiences]  WITH CHECK ADD  CONSTRAINT [FK_ImageCultureExperiences_CultureExperiences_CultureExperienceID] FOREIGN KEY([CultureExperienceID])
REFERENCES [dbo].[CultureExperiences] ([CultureExperienceID])
GO
ALTER TABLE [dbo].[ImageCultureExperiences] CHECK CONSTRAINT [FK_ImageCultureExperiences_CultureExperiences_CultureExperienceID]
GO
ALTER TABLE [dbo].[ImageHomeStayRentals]  WITH CHECK ADD  CONSTRAINT [FK_ImageHomeStayRentals_HomeStayRentals_HomeStayRentalID] FOREIGN KEY([HomeStayRentalID])
REFERENCES [dbo].[HomeStayRentals] ([HomeStayRentalID])
GO
ALTER TABLE [dbo].[ImageHomeStayRentals] CHECK CONSTRAINT [FK_ImageHomeStayRentals_HomeStayRentals_HomeStayRentalID]
GO
ALTER TABLE [dbo].[ImageRoomTypes]  WITH CHECK ADD  CONSTRAINT [FK_ImageRoomTypes_RoomTypes_RoomTypesID] FOREIGN KEY([RoomTypesID])
REFERENCES [dbo].[RoomTypes] ([RoomTypesID])
GO
ALTER TABLE [dbo].[ImageRoomTypes] CHECK CONSTRAINT [FK_ImageRoomTypes_RoomTypes_RoomTypesID]
GO
ALTER TABLE [dbo].[ImageServices]  WITH CHECK ADD  CONSTRAINT [FK_ImageServices_Services_ServicesID] FOREIGN KEY([ServicesID])
REFERENCES [dbo].[Services] ([ServicesID])
GO
ALTER TABLE [dbo].[ImageServices] CHECK CONSTRAINT [FK_ImageServices_Services_ServicesID]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_AspNetUsers_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_AspNetUsers_AccountID]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Bookings_BookingID] FOREIGN KEY([BookingID])
REFERENCES [dbo].[Bookings] ([BookingID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Bookings_BookingID]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_BookingServices_BookingServicesID] FOREIGN KEY([BookingServicesID])
REFERENCES [dbo].[BookingServices] ([BookingServicesID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_BookingServices_BookingServicesID]
GO
ALTER TABLE [dbo].[Prices]  WITH CHECK ADD  CONSTRAINT [FK_Prices_HomeStayRentals_HomeStayRentalID] FOREIGN KEY([HomeStayRentalID])
REFERENCES [dbo].[HomeStayRentals] ([HomeStayRentalID])
GO
ALTER TABLE [dbo].[Prices] CHECK CONSTRAINT [FK_Prices_HomeStayRentals_HomeStayRentalID]
GO
ALTER TABLE [dbo].[Prices]  WITH CHECK ADD  CONSTRAINT [FK_Prices_RoomTypes_RoomTypesID] FOREIGN KEY([RoomTypesID])
REFERENCES [dbo].[RoomTypes] ([RoomTypesID])
GO
ALTER TABLE [dbo].[Prices] CHECK CONSTRAINT [FK_Prices_RoomTypes_RoomTypesID]
GO
ALTER TABLE [dbo].[Rating]  WITH CHECK ADD  CONSTRAINT [FK_Rating_AspNetUsers_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Rating] CHECK CONSTRAINT [FK_Rating_AspNetUsers_AccountID]
GO
ALTER TABLE [dbo].[Rating]  WITH CHECK ADD  CONSTRAINT [FK_Rating_HomeStays_HomeStayID] FOREIGN KEY([HomeStayID])
REFERENCES [dbo].[HomeStays] ([HomeStayID])
GO
ALTER TABLE [dbo].[Rating] CHECK CONSTRAINT [FK_Rating_HomeStays_HomeStayID]
GO
ALTER TABLE [dbo].[RefreshTokens]  WITH CHECK ADD  CONSTRAINT [FK_RefreshTokens_AspNetUsers_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RefreshTokens] CHECK CONSTRAINT [FK_RefreshTokens_AspNetUsers_AccountID]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_AspNetUsers_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_AspNetUsers_AccountID]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_HomeStays_HomeStayID] FOREIGN KEY([HomeStayID])
REFERENCES [dbo].[HomeStays] ([HomeStayID])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_HomeStays_HomeStayID]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK_Reviews_AspNetUsers_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK_Reviews_AspNetUsers_AccountID]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK_Reviews_HomeStays_HomeStayID] FOREIGN KEY([HomeStayID])
REFERENCES [dbo].[HomeStays] ([HomeStayID])
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK_Reviews_HomeStays_HomeStayID]
GO
ALTER TABLE [dbo].[Rooms]  WITH CHECK ADD  CONSTRAINT [FK_Rooms_RoomTypes_RoomTypesID] FOREIGN KEY([RoomTypesID])
REFERENCES [dbo].[RoomTypes] ([RoomTypesID])
GO
ALTER TABLE [dbo].[Rooms] CHECK CONSTRAINT [FK_Rooms_RoomTypes_RoomTypesID]
GO
ALTER TABLE [dbo].[RoomTypes]  WITH CHECK ADD  CONSTRAINT [FK_RoomTypes_HomeStayRentals_HomeStayRentalID] FOREIGN KEY([HomeStayRentalID])
REFERENCES [dbo].[HomeStayRentals] ([HomeStayRentalID])
GO
ALTER TABLE [dbo].[RoomTypes] CHECK CONSTRAINT [FK_RoomTypes_HomeStayRentals_HomeStayRentalID]
GO
ALTER TABLE [dbo].[Services]  WITH CHECK ADD  CONSTRAINT [FK_Services_HomeStays_HomeStayID] FOREIGN KEY([HomeStayID])
REFERENCES [dbo].[HomeStays] ([HomeStayID])
GO
ALTER TABLE [dbo].[Services] CHECK CONSTRAINT [FK_Services_HomeStays_HomeStayID]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_Bookings_BookingID] FOREIGN KEY([BookingID])
REFERENCES [dbo].[Bookings] ([BookingID])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_Bookings_BookingID]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_BookingServices_BookingServicesID] FOREIGN KEY([BookingServicesID])
REFERENCES [dbo].[BookingServices] ([BookingServicesID])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_BookingServices_BookingServicesID]
GO
