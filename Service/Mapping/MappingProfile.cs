using AutoMapper;
using BusinessObject.Model;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.ImageHomeStayTypes;
using Service.RequestAndResponse.Request.ImageService;
<<<<<<< HEAD
using Service.RequestAndResponse.Request.Location;
using Service.RequestAndResponse.Request.Properties;
using Service.RequestAndResponse.Request.Province;
using Service.RequestAndResponse.Request.Report;
=======
using Service.RequestAndResponse.Request.RoomType;
>>>>>>> main
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.BookingOfServicesDetails;
using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.ImageService;
<<<<<<< HEAD
using Service.RequestAndResponse.Response.Locations;
using Service.RequestAndResponse.Response.Properties;
using Service.RequestAndResponse.Response.Provinces;
using Service.RequestAndResponse.Response.Reports;
=======
using Service.RequestAndResponse.Response.RoomType;
>>>>>>> main
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
            /*CreateMap<Street, GetStreet>();*/
            CreateMap<CreateHomeStayRequest, HomeStay>().ReverseMap();
            CreateMap<UpdateHomeStayRequest, HomeStay>().ReverseMap();

<<<<<<< HEAD
            CreateMap<HomeStayTypes, GetAllHomeStayType>();
            CreateMap<CreateHomeStayTypeRequest, HomeStayTypes>().ReverseMap();
            CreateMap<UpdateHomeStayTypeRequest, HomeStayTypes>().ReverseMap();
=======
            CreateMap<HomeStayRentals, GetAllHomeStayType>();
            CreateMap<CreateHomeStayTypeRequest, HomeStayRentals>().ReverseMap();
>>>>>>> main

            CreateMap<CreateRoomTypeRequest, RoomTypes>().ReverseMap();
            CreateMap<GetAllRoomType, RoomTypes>().ReverseMap();

           

            CreateMap<Services, GetAllServices>();
            CreateMap<Services, CreateServices>().ReverseMap();
<<<<<<< HEAD
            CreateMap<Services, UpdateServices>().ReverseMap();
            CreateMap<Report, GetAllReport>();
            CreateMap<Report, CreateReport>().ReverseMap();
            CreateMap<Report, UpdateReport>().ReverseMap();
            CreateMap<ImageHomeStayTypes, AddImageHomeStayTypesRequest>().ReverseMap();
            CreateMap<ImageHomeStayTypes, UpdateImageHomeStayTypesRequest>().ReverseMap();

            CreateMap<ImageHomeStayTypes, GetAllImageHomeStayType>();
=======
            CreateMap<ImageHomeStayRentals, AddImageHomeStayTypesRequest>().ReverseMap();
            CreateMap<ImageHomeStayRentals, UpdateImageHomeStayTypesRequest>().ReverseMap();
            CreateMap<ImageHomeStayRentals, GetAllImageHomeStayType>();
>>>>>>> main
            CreateMap<BookingDetail, GetBookingDetails>();
            CreateMap<BookingServices, GetAllBookingServices>();
            CreateMap<Booking, GetAllBookings>();
            CreateMap<BookingServicesDetail, GetAllDetailOfServices>();


            

            
            CreateMap<ImageServices, GetAllImageService>();
            CreateMap<ImageServices, GetImageService>();
            CreateMap<ImageServices, AddImageServicesRequest>().ReverseMap();
            CreateMap<ImageServices, UpdateImageServicesRequest>().ReverseMap();
        }
    }
}
