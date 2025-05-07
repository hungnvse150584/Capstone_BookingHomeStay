using AutoMapper;
using BusinessObject.Model;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.BookingDetail;
using Service.RequestAndResponse.Request.CancellationPolicy;
using Service.RequestAndResponse.Request.CommissionRates;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.ImageHomeStay;
using Service.RequestAndResponse.Request.ImageHomeStayTypes;
using Service.RequestAndResponse.Request.ImageService;
using Service.RequestAndResponse.Request.Notifications;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Request.Rating;
using Service.RequestAndResponse.Request.Room;
using Service.RequestAndResponse.Request.RoomChangeHistory;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Request.Staffs;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.BookingOfServicesDetails;
using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.CancellationPolicyRequest;
using Service.RequestAndResponse.Response.CommissionRate;
using Service.RequestAndResponse.Response.Conversation;
using Service.RequestAndResponse.Response.CultureExperiences;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.ImageHomeStay;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.ImageRating;
using Service.RequestAndResponse.Response.ImageService;
using Service.RequestAndResponse.Response.Messages;
using Service.RequestAndResponse.Response.Notifications;
using Service.RequestAndResponse.Response.Pricing;
using Service.RequestAndResponse.Response.Ratings;
using Service.RequestAndResponse.Response.Reports;
using Service.RequestAndResponse.Response.Room;
using Service.RequestAndResponse.Response.RoomType;
using Service.RequestAndResponse.Response.Services;
using Service.RequestAndResponse.Response.Staffs;
using Service.RequestAndResponse.Response.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Service.RequestAndResponse.Request.RoomType.CreateRoomTypeRequest;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Service.Mapping
{
    public class MappingProfile : Profile
    {
        private decimal? GetRentPriceFromPrices(ICollection<Pricing> prices)
        {
            if (prices == null || !prices.Any()) return null;
            var pricing = prices.FirstOrDefault(p => p.DayType == DayType.Weekday && p.IsActive)
                         ?? prices.FirstOrDefault(p => p.IsActive);
            return pricing != null ? (decimal?)pricing.RentPrice : null;
        }
        private decimal? GetDefaultRentPriceFromPrices(ICollection<Pricing> prices)
        {
            if (prices == null) return null;
            // Ưu tiên lấy giá IsDefault và IsActive
            var defaultPricing = prices
                .Where(p => p.IsDefault && p.IsActive)
                .OrderBy(p => p.RentPrice)
                .FirstOrDefault();
            if (defaultPricing != null)
            {
                return (decimal?)defaultPricing.RentPrice;
            }
            // Nếu không có giá IsDefault, lấy giá rẻ nhất từ tất cả Prices với IsActive = true
            var cheapestPricing = prices
                .Where(p => p.IsActive)
                .OrderBy(p => p.RentPrice)
                .FirstOrDefault();
            return cheapestPricing != null ? (decimal?)cheapestPricing.RentPrice : null;
        }
        public MappingProfile()
        {
            CreateMap<HomeStay, HomeStayResponse>()
      .ForMember(dest => dest.CommissionRateID, opt => opt.MapFrom(src => src.CommissionRateID))
      .ForMember(dest => dest.SumRate, opt => opt.MapFrom(src => src.Ratings.Any() ? src.Ratings.Average(r => r.SumRate) : (double?)null))
      .ForMember(dest => dest.DefaultRentPrice, opt => opt.MapFrom(src =>
          src.HomeStayRentals != null && src.HomeStayRentals.Any()
              ? src.HomeStayRentals.SelectMany(r => r.RoomTypes ?? new List<RoomTypes>())
                  .SelectMany(rt => rt.Prices ?? new List<Pricing>())
                  .Where(p => p.IsActive)
                  .OrderBy(p => p.RentPrice)
                  .Select(p => (decimal?)p.RentPrice)
                  .FirstOrDefault()
              : null));


            CreateMap<HomeStay, SimpleHomeStayResponse>()
     .ForMember(dest => dest.SumRate, opt => opt.MapFrom(src =>
         src.Ratings != null && src.Ratings.Any(r => r.HomeStayID.HasValue && r.HomeStayID == src.HomeStayID)
             ? src.Ratings.Where(r => r.HomeStayID.HasValue && r.HomeStayID == src.HomeStayID).Average(r => r.SumRate)
             : (double?)null))
     .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src =>
         src.Ratings != null ? src.Ratings.Count(r => r.HomeStayID.HasValue && r.HomeStayID == src.HomeStayID) : 0))
     .ForMember(dest => dest.LatestRatings, opt => opt.MapFrom(src =>
         src.Ratings != null && src.Ratings.Any(r => r.HomeStayID.HasValue && r.HomeStayID == src.HomeStayID)
             ? src.Ratings.Where(r => r.HomeStayID.HasValue && r.HomeStayID == src.HomeStayID)
                 .OrderByDescending(r => r.CreatedAt)
                 .Take(5)
                 .Select(r => new CreateRatingResponse
                 {
                     RatingID = r.RatingID,
                     SumRate = r.SumRate,
                     CleaningRate = r.CleaningRate,
                     ServiceRate = r.ServiceRate,
                     FacilityRate = r.FacilityRate,
                     Content = r.Content,
                     AccountID = r.AccountID,
                     Username = r.Account != null ? r.Account.UserName : null,
                     HomeStayID = r.HomeStayID ?? 0,
                     BookingID = r.BookingID ?? 0,
                     CreatedAt = r.CreatedAt,
                     UpdatedAt = r.UpdatedAt,
                     ImageRatings = r.ImageRatings != null
                         ? r.ImageRatings.Select(ir => new ImageRatingResponse
                         {
                             ImageRatingID = ir.ImageRatingID,
                             Image = ir.Image,
                             RatingID = ir.RatingID ?? 0
                         }).ToList()
                         : new List<ImageRatingResponse>()
                 }).ToList()
             : new List<CreateRatingResponse>()))
     .ForMember(dest => dest.CheapestPrice, opt => opt.MapFrom(src =>
         src.HomeStayRentals != null && src.HomeStayRentals.Any()
             ? src.HomeStayRentals.SelectMany(r => r.RoomTypes ?? new List<RoomTypes>())
                 .SelectMany(rt => rt.Prices ?? new List<Pricing>())
                 .Where(p => p.IsActive && p.RoomTypesID.HasValue)
                 .OrderBy(p => p.RentPrice)
                 .Select(p => (decimal?)p.RentPrice)
                 .FirstOrDefault()
             : null));
            CreateMap<ImageHomeStay, ImageHomeStayResponse>().ReverseMap();

            CreateMap<CreateHomeStayRequest, HomeStay>().ReverseMap();
            CreateMap<UpdateHomeStayRequest, HomeStay>().ReverseMap();
            CreateMap<UploadImageRequest, ImageHomeStay>().ReverseMap();
            CreateMap<UpdateHomeStayImagesBodyRequest, ImageHomeStay>().ReverseMap();

            CreateMap<Account, GetAccountUser>().ReverseMap();
            CreateMap<CreateStaffRequest, Staff>().ReverseMap();
            CreateMap<Staff, GetAllStaff>()
                .ForMember(dest => dest.HomeStay, opt => opt.MapFrom(src => src.HomeStay));

            CreateMap<RoomTypes, GetAllRoomType>().ReverseMap();
            CreateMap<ImageHomeStayRentals, GetAllImageHomeStayType>().ReverseMap();
            CreateMap<Conversation, ConversationResponse>();
            // Ánh xạ HomeStayRentals sang GetAllHomeStayType
            CreateMap<HomeStayRentals, GetAllHomeStayType>()
                .ForMember(dest => dest.ImageHomeStayRentals, opt => opt.MapFrom(src => src.ImageHomeStayRentals))
                .ForMember(dest => dest.RoomTypes, opt => opt.MapFrom(src => src.RoomTypes))
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails))
                .ForMember(dest => dest.Pricing, opt => opt.MapFrom(src => src.Prices));

            // Ánh xạ HomeStayRentals sang GetAllHomeStayTypeFilter
            CreateMap<HomeStayRentals, GetAllHomeStayTypeFilter>()
                .ForMember(dest => dest.ImageHomeStayRentals, opt => opt.MapFrom(src => src.ImageHomeStayRentals))
                .ForMember(dest => dest.RoomTypes, opt => opt.MapFrom(src => src.RoomTypes))
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails))
                .ForMember(dest => dest.Pricing, opt => opt.MapFrom(src => src.Prices));

            // Ánh xạ các kiểu con cho GetAllHomeStayTypeFilter
            CreateMap<ImageHomeStayRentals, GetAllImageHomeStayType>().ReverseMap();

            CreateMap<RoomTypes, GetAllRoomTypeForFilter>()
                .ForMember(dest => dest.ImageRoomTypes, opt => opt.MapFrom(src => src.ImageRoomTypes))
                .ForMember(dest => dest.Pricings, opt => opt.MapFrom(src => src.Prices))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.Rooms));

            // Sửa ánh xạ cho BookingDetail sang GetBookingDetailFilter
            CreateMap<BookingDetail, GetBookingDetailFilter>()
                .ForMember(dest => dest.rentPrice, opt => opt.MapFrom(src => src.rentPrice))
                .ForMember(dest => dest.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate))
                .ForMember(dest => dest.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate))
                .ForMember(dest => dest.Booking, opt => opt.MapFrom(src => src.Booking));

            // Ánh xạ Booking sang GetAllBookings
            CreateMap<Booking, GetAllBookings>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Transactions, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore());

            CreateMap<Pricing, GetAllPricing>().ReverseMap();
            CreateMap<Room, GetAllRooms>()
                .ForMember(dest => dest.isActive, opt => opt.MapFrom(src => src.isActive));

            // Thêm ánh xạ từ Room sang GetRoomResponse
            CreateMap<Room, GetRoomResponse>().ReverseMap();
            CreateMap<Room, GetRoomsResponse>().ReverseMap();


            // Ánh xạ từ Room sang GetAllRooms
            CreateMap<Room, GetAllRooms>()
              .ForMember(dest => dest.RoomID, opt => opt.MapFrom(src => src.RoomID))
              .ForMember(dest => dest.roomNumber, opt => opt.MapFrom(src => src.roomNumber))
              .ForMember(dest => dest.isActive, opt => opt.MapFrom(src => src.isActive))
              .ForMember(dest => dest.RoomTypesID, opt => opt.MapFrom(src => src.RoomTypesID))
              .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomTypes != null ? src.RoomTypes.Name : null))
              .ForMember(dest => dest.RentPrice, opt => opt.MapFrom(src => src.RoomTypes != null && src.RoomTypes.Prices != null ? GetRentPriceFromPrices(src.RoomTypes.Prices) : null));

            CreateMap<Booking, GetAllBookings>().ReverseMap();
            CreateMap<ImageRoomTypes, ImageRoomTypeResponse>().ReverseMap();
            CreateMap<Pricing, PricingForHomeStayRental>().ReverseMap();

            CreateMap<CreateHomeStayTypeRequest, HomeStayRentals>()
                .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => src.PricingJson));

            CreateMap<HomeStayRentals, GetSimpleHomeStayType>().ReverseMap();

            CreateMap<HomeStayRentals, GetHomeStayRentalDetailResponse>()
                .ForMember(dest => dest.Pricing, opt => opt.MapFrom(src => src.Prices))
                .ForMember(dest => dest.ImageHomeStayRentals, opt => opt.MapFrom(src => src.ImageHomeStayRentals))
                .ForMember(dest => dest.RoomTypes, opt => opt.MapFrom(src => src.RoomTypes))
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails));

            CreateMap<RoomTypes, RoomTypeDetailResponse>()
                .ForMember(dest => dest.Pricings, opt => opt.MapFrom(src => src.Prices))
                .ForMember(dest => dest.ImageRoomTypes, opt => opt.MapFrom(src => src.ImageRoomTypes));


            CreateMap<CreateHomeStayTypeRequest, HomeStayRentals>()
                .ForMember(dest => dest.numberBedRoom, opt => opt.MapFrom(src => src.numberBedRoom))
                .ForMember(dest => dest.numberBathRoom, opt => opt.MapFrom(src => src.numberBathRoom))
                .ForMember(dest => dest.numberKitchen, opt => opt.MapFrom(src => src.numberKitchen))
                .ForMember(dest => dest.numberWifi, opt => opt.MapFrom(src => src.numberWifi))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? true))
                .ForMember(dest => dest.RentWhole, opt => opt.MapFrom(src => src.RentWhole ?? true))
                .ForMember(dest => dest.HomeStayRentalID, opt => opt.Ignore())
                .ForMember(dest => dest.HomeStay, opt => opt.Ignore())
                .ForMember(dest => dest.ImageHomeStayRentals, opt => opt.Ignore())
                .ForMember(dest => dest.Prices, opt => opt.Ignore())
                .ForMember(dest => dest.BookingDetails, opt => opt.Ignore())
                .ForMember(dest => dest.RoomTypes, opt => opt.Ignore());

            CreateMap<GetAllRoomType, RoomTypes>().ReverseMap();
            CreateMap<RoomTypes, GetSingleRoomType>()
                .ForMember(dest => dest.Pricings, opt => opt.MapFrom(src => src.Prices))
                .ForMember(dest => dest.ImageRoomTypes, opt => opt.MapFrom(src => src.ImageRoomTypes));

            CreateMap<RoomTypes, GetAllRoomTypeByRental>()
                .ForMember(dest => dest.Pricings, opt => opt.MapFrom(src => src.Prices))
                .ForMember(dest => dest.ImageRoomTypes, opt => opt.MapFrom(src => src.ImageRoomTypes));

            CreateMap<PricingForHomeStayRental, Pricing>()
                .ForMember(dest => dest.PricingID, opt => opt.Ignore())
                .ForMember(dest => dest.HomeStayRentalID, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.RentPrice, opt => opt.MapFrom(src => src.RentPrice))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.DayType, opt => opt.MapFrom(src => src.DayType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? "Default description"));

            CreateMap<Pricing, GetPricing>()
                .ForMember(dest => dest.PricingID, opt => opt.MapFrom(src => src.PricingID))
                .ForMember(dest => dest.HomeStayRentalID, opt => opt.MapFrom(src => src.HomeStayRentalID))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.RentPrice, opt => opt.MapFrom(src => src.RentPrice))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.DayType, opt => opt.MapFrom(src => src.DayType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? "Default description"));

            CreateMap<GetAllPricing, Pricing>().ReverseMap();
            CreateMap<Pricing, PricingResponse>();
            CreateMap<CreatePricingRequest, Pricing>().ReverseMap();
            CreateMap<UpdatePricingRequest, Pricing>().ReverseMap();

            CreateMap<CreateRoomTypeRequest, RoomTypes>()
                .ForMember(dest => dest.RoomTypesID, opt => opt.Ignore())
                .ForMember(dest => dest.CreateAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.Ignore())
                .ForMember(dest => dest.HomeStayRentalID, opt => opt.Ignore())
                .ForMember(dest => dest.ImageRoomTypes, opt => opt.Ignore())
                .ForMember(dest => dest.Prices, opt => opt.Ignore())
                .ForMember(dest => dest.Rooms, opt => opt.Ignore());

            CreateMap<RoomTypes, CreateRoomTypeResponse>()
                .ForMember(dest => dest.ImageRoomTypes, opt => opt.MapFrom(src => src.ImageRoomTypes))
                .ForMember(dest => dest.Pricings, opt => opt.MapFrom(src => src.Prices));
            CreateMap<UpdateRoomTypeRequest, RoomTypes>().ReverseMap();


            CreateMap<PricingForHomeStayRental, Pricing>()
                .ForMember(dest => dest.PricingID, opt => opt.Ignore())
                .ForMember(dest => dest.RoomTypesID, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.RentPrice, opt => opt.MapFrom(src => src.RentPrice))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.DayType, opt => opt.MapFrom(src => src.DayType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? "Default description"));

            CreateMap<Pricing, PricingForHomeStayRental>();

            CreateMap<HomeStayRentals, GetHomeStayRentalDetailResponse>()
                .ForMember(dest => dest.Pricing, opt => opt.MapFrom(src => src.Prices))
                .ForMember(dest => dest.ImageHomeStayRentals, opt => opt.MapFrom(src => src.ImageHomeStayRentals))
                .ForMember(dest => dest.RoomTypes, opt => opt.MapFrom(src => src.RoomTypes))
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails));

            CreateMap<CreateRoomRequest, Room>().ReverseMap();
         
            CreateMap<CreateRoomRequest, Room>().ReverseMap();
            CreateMap<UpdateRoomRequest, Room>().ReverseMap();

            CreateMap<GetAllCommissionRate, CommissionRate>().ReverseMap();
            CreateMap<CreateCommissionRateRequest, CommissionRate>().ReverseMap();
            CreateMap<CommissionRate, UpdateCommissionRateRequest>().ReverseMap();

            CreateMap<CancellationPolicy, CreateCancellationPolicyRequest>().ReverseMap();
            CreateMap<CancellationPolicy, UpdateCancellationPolicyRequest>().ReverseMap();
            CreateMap<CancellationPolicy, GetAllCancellationPolicy>().ReverseMap();

            CreateMap<ImageServices, GetAllImageService>().ReverseMap();
            CreateMap<Services, GetAllServices>().ReverseMap();
            CreateMap<Services, CreateServices>().ReverseMap();
            CreateMap<UpdateServices, Services>()
                .ForMember(dest => dest.ServicesID, opt => opt.Ignore());

            CreateMap<ImageHomeStayRentals, AddImageHomeStayTypesRequest>().ReverseMap();
            CreateMap<ImageHomeStayRentals, UpdateImageHomeStayTypesRequest>().ReverseMap();
            CreateMap<ImageHomeStayRentals, GetAllImageHomeStayType>();

            CreateMap<BookingDetail, GetBookingDetails>();
            CreateMap<BookingDetail, GetBookingDetailFilter>()
                .ForMember(dest => dest.rentPrice, opt => opt.MapFrom(src => src.rentPrice));

            CreateMap<BookingServices, GetAllBookingServices>();
            CreateMap<BookingServices, GetSimpleBookingService>();
            CreateMap<Booking, GetAllBookings>();
            CreateMap<Booking, GetBookingResponse>().ReverseMap();

            CreateMap<BookingDetail, GetBookingDetailForRoom>().ReverseMap();
            CreateMap<BookingDetail, GetBookingDetailResponse>().ReverseMap();
            CreateMap<BookingDetail, GetBookingDetails>().ReverseMap();
            CreateMap<BookingDetail, GetSimpleBookingDetail>().ReverseMap();
            CreateMap<BookingServicesDetail, GetSimpleDetailOfService>().ReverseMap();
            CreateMap<BookingServicesDetail, GetSingleDetailOfService>().ReverseMap();
            CreateMap<BookingServicesDetail, SingleServiceDetail>().ReverseMap();
            CreateMap<BookingServices, GetAllBookingServices>().ReverseMap();
            CreateMap<BookingServices, GetSimpleBookingService>().ReverseMap();
            CreateMap<BookingServices, GetBookingServiceByAccount>().ReverseMap();
            CreateMap<BookingServices, GetBookingServiceByHomeStay>().ReverseMap();
            CreateMap<BookingServices, GetBookingService>().ReverseMap();
            CreateMap<BookingServices, GetBookingServiceResponse>().ReverseMap();
            CreateMap<Booking, GetAllBookings>().ReverseMap();

            CreateMap<Booking, GetCancellationBooking>();
            CreateMap<Booking, GetBookingByAccount>().ReverseMap();
            CreateMap<Booking, GetBookingByHomeStay>().ReverseMap();
            CreateMap<Booking, GetBookingByRoom>().ReverseMap();
            CreateMap<Booking, GetSimpleBooking>().ReverseMap();
            //Cho phép đổi phòng khi có sự cố
            CreateMap<BookingDetail, UpdateChangingRoomRequest>().ReverseMap();
            CreateMap<RoomChangeHistory, CreateRoomChagingRequest>().ReverseMap();
            CreateMap<Booking, UpdateBookingForRoomRequest>().ReverseMap();


            CreateMap<ImageServices, GetAllImageService>();
            CreateMap<ImageServices, GetImageService>();
            CreateMap<ImageServices, UploadImageServiceRequest>().ReverseMap();
            CreateMap<ImageServices, UpdateImageServicesRequest>().ReverseMap();
            CreateMap<BookingServicesDetail, GetAllDetailOfServices>();
            CreateMap<ImageHomeStay, ImageHomeStayResponse>().ReverseMap();
            CreateMap<Rating, GetAllRatingResponse>().ReverseMap();
            CreateMap<Report, GetReportResponse>().ReverseMap();
            CreateMap<CultureExperience, GetAllCultureExperiencesResponse>().ReverseMap();
            CreateMap<Services, GetServiceForHomeStay>().ReverseMap();
            CreateMap<Services, GetSingleService>().ReverseMap();
            CreateMap<Services, GetServiceForAccount>().ReverseMap();
            CreateMap<HomeStay, SimpleHomeStayResponse>().ReverseMap();
            CreateMap<HomeStay, GetHomeStayResponse>();
            CreateMap<HomeStay, GetAllHomeStayWithOwnerName>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Account.Name));

            CreateMap<Conversation, ConversationResponse>()
                .ForMember(dest => dest.OtherUser, opt => opt.Ignore())
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()));

            CreateMap<Message, MessageResponse>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.Name : null));

            CreateMap<Conversation, SimplifiedConversationResponse>()
                .ForMember(dest => dest.ConversationID, opt => opt.MapFrom(src => src.ConversationID))
                .ForMember(dest => dest.OtherUser, opt => opt.Ignore()) // Sẽ xử lý thủ công trong controller
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()));

            // Ánh xạ từ Account sang SimplifiedAccountResponse
            CreateMap<Account, SimplifiedAccountResponse>()
                .ForMember(dest => dest.AccountID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            CreateMap<CancellationPolicy, GetAllCancellationPolicy>()
                .ForMember(dest => dest.CancellationID, opt => opt.MapFrom(src => src.CancellationID))
                .ForMember(dest => dest.DayBeforeCancel, opt => opt.MapFrom(src => src.DayBeforeCancel))
                .ForMember(dest => dest.RefundPercentage, opt => opt.MapFrom(src => src.RefundPercentage))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => src.UpdateAt))
                .ForMember(dest => dest.HomeStayID, opt => opt.MapFrom(src => src.HomeStayID));
            CreateMap<HomeStay, SimpleHomeStayResponse>()
                .ForMember(dest => dest.CancelPolicy, opt => opt.MapFrom(src => src.CancelPolicy));
            // Ánh xạ từ Message sang SimplifiedMessageResponse
            CreateMap<Message, SimplifiedMessageResponse>()
                .ForMember(dest => dest.MessageID, opt => opt.MapFrom(src => src.MessageID))
                .ForMember(dest => dest.SenderID, opt => opt.MapFrom(src => src.SenderID))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead));
            CreateMap<Notification, NotificationResponse>()
            .ForMember(dest => dest.NotificationID, opt => opt.MapFrom(src => src.NotificationID))
            .ForMember(dest => dest.AccountID, opt => opt.MapFrom(src => src.AccountID))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt))
            .ForMember(dest => dest.BookingID, opt => opt.MapFrom(src => src.BookingID))
            .ForMember(dest => dest.BookingServicesID, opt => opt.MapFrom(src => src.BookingServicesID))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.TypeNotify, opt => opt.MapFrom(src => src.TypeNotify));

            CreateMap<CreateNotificationRequest, Notification>()
                .ForMember(dest => dest.AccountID, opt => opt.MapFrom(src => src.AccountID))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.BookingID, opt => opt.MapFrom(src => src.BookingID))
                .ForMember(dest => dest.BookingServicesID, opt => opt.MapFrom(src => src.BookingServicesID))
                .ForMember(dest => dest.TypeNotify, opt => opt.MapFrom(src => src.TypeNotify));

            CreateMap<Transaction, TransactionResponse>()
                .ForMember(dest => dest.TmnCode, opt => opt.MapFrom(src => src.TmnCode))
                .ForMember(dest => dest.TxnRef, opt => opt.MapFrom(src => src.TxnRef))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.OrderInfo, opt => opt.MapFrom(src => src.OrderInfo))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.BankTranNo, opt => opt.MapFrom(src => src.BankTranNo))
                .ForMember(dest => dest.PayDate, opt => opt.MapFrom(src => src.PayDate))
                .ForMember(dest => dest.BankCode, opt => opt.MapFrom(src => src.BankCode))
                .ForMember(dest => dest.TransactionNo, opt => opt.MapFrom(src => src.TransactionNo))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType))
                .ForMember(dest => dest.TransactionStatus, opt => opt.MapFrom(src => src.TransactionStatus))
                .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.Account)) // assuming you have CreateMap<Account, GetAccountUser>()
                .ForMember(dest => dest.HomeStay, opt => opt.MapFrom(src => src.HomeStay)) // assuming you have CreateMap<HomeStay, HomeStayResponse>()
                .ForMember(dest => dest.BookingID, opt => opt.MapFrom(src => src.Booking != null ? src.Booking.BookingID : 0))
                .ForMember(dest => dest.BookingServicesID, opt => opt.MapFrom(src => src.BookingService != null ? src.BookingService.BookingServicesID : 0));
            CreateMap<CreateServices, Services>()
            .ForMember(dest => dest.servicesName, opt => opt.MapFrom(src => src.servicesName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.servicesPrice, opt => opt.MapFrom(src => src.servicesPrice))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.HomeStayID, opt => opt.MapFrom(src => src.HomeStayID))
            .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType));

            CreateMap<CreateServices, Services>()
             .ForMember(dest => dest.servicesName, opt => opt.MapFrom(src => src.servicesName))
             .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
             .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
             .ForMember(dest => dest.servicesPrice, opt => opt.MapFrom(src => src.servicesPrice))
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
             .ForMember(dest => dest.HomeStayID, opt => opt.MapFrom(src => src.HomeStayID))
             .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
             .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity)); // Thêm ánh xạ cho Quantity

            CreateMap<UpdateServices, Services>()
                .ForMember(dest => dest.servicesName, opt => opt.MapFrom(src => src.servicesName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.servicesPrice, opt => opt.MapFrom(src => src.servicesPrice))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.HomeStayID, opt => opt.MapFrom(src => src.HomeStayID))
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
            CreateMap<CreateRatingRequest, Rating>().ReverseMap();
            CreateMap<UpdateRatingRequest, Rating>().ReverseMap();
            CreateMap<ImageRating, ImageRatingResponse>()
                 .ForMember(dest => dest.ImageRatingID, opt => opt.MapFrom(src => src.ImageRatingID))
                 .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                 .ForMember(dest => dest.RatingID, opt => opt.MapFrom(src => src.RatingID));

            CreateMap<Rating, CreateRatingResponse>()
                .ForMember(dest => dest.RatingID, opt => opt.MapFrom(src => src.RatingID))
                .ForMember(dest => dest.SumRate, opt => opt.MapFrom(src => src.SumRate))
                .ForMember(dest => dest.CleaningRate, opt => opt.MapFrom(src => src.CleaningRate))
                .ForMember(dest => dest.ServiceRate, opt => opt.MapFrom(src => src.ServiceRate))
                .ForMember(dest => dest.FacilityRate, opt => opt.MapFrom(src => src.FacilityRate))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.AccountID, opt => opt.MapFrom(src => src.AccountID))
                  .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Account != null ? src.Account.UserName : null))
                .ForMember(dest => dest.HomeStayID, opt => opt.MapFrom(src => src.HomeStayID))
                .ForMember(dest => dest.BookingID, opt => opt.MapFrom(src => src.BookingID))
                .ForMember(dest => dest.ImageRatings, opt => opt.MapFrom(src => src.ImageRatings));
        }
      
    }

}