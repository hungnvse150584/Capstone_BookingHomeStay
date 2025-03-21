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

            CreateMap<CreateHomeStayTypeRequest, HomeStayRentals>().ReverseMap();

            CreateMap<CreateRoomTypeRequest, RoomTypes>().ReverseMap();
            CreateMap<GetAllRoomType, RoomTypes>().ReverseMap();

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
