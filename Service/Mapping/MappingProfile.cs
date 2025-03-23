using AutoMapper;
using BusinessObject.Model;
using Service.RequestAndResponse.Request.CancellationPolicy;
using Service.RequestAndResponse.Request.CommissionRates;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.ImageHomeStay;
using Service.RequestAndResponse.Request.ImageHomeStayTypes;
using Service.RequestAndResponse.Request.ImageService;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Request.Room;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.BookingOfServicesDetails;
using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.CancellationPolicyRequest;
using Service.RequestAndResponse.Response.CommissionRate;
using Service.RequestAndResponse.Response.CultureExperiences;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.ImageHomeStay;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.ImageService;
using Service.RequestAndResponse.Response.Pricing;
using Service.RequestAndResponse.Response.Ratings;
using Service.RequestAndResponse.Response.Reports;
using Service.RequestAndResponse.Response.Room;
using Service.RequestAndResponse.Response.RoomType;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<HomeStay, HomeStayResponse>();
            //CreateMap<HomeStay, SimpleHomeStayResponse>().ReverseMap();
            /*CreateMap<Street, GetStreet>();*/
            CreateMap<CreateHomeStayRequest, HomeStay>().ReverseMap();
            CreateMap<UpdateHomeStayRequest, HomeStay>().ReverseMap();
            CreateMap<UploadImageRequest, ImageHomeStay>().ReverseMap();


            
            CreateMap<RoomTypes, GetAllRoomType>().ReverseMap();
            CreateMap<ImageHomeStayRentals, GetAllImageHomeStayType>().ReverseMap();
            CreateMap<HomeStayRentals, GetAllHomeStayType>()
        .ForMember(dest => dest.ImageHomeStayRentals,
               opt => opt.MapFrom(src => src.ImageHomeStayRentals));

            CreateMap<CreateHomeStayTypeRequest, HomeStayRentals>()
    .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => src.PricingJson));


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
            CreateMap<GetAllPricing, Pricing>().ReverseMap();
            CreateMap<CreatePricingRequest, Pricing>().ReverseMap();
            CreateMap<UpdatePricingRequest, Pricing>().ReverseMap();

            CreateMap<GetAllRooms, Room>().ReverseMap();
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
            CreateMap<BookingServices, GetAllBookingServices>();
            CreateMap<Booking, GetAllBookings>();
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
            CreateMap<HomeStay, SimpleHomeStayResponse>().ReverseMap();
            CreateMap<HomeStay, GetAllHomeStayWithOwnerName>()
    .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Account.Name));



        }







    }
}
