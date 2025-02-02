using AutoMapper;
using BusinessObject.Model;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.Location;
using Service.RequestAndResponse.Request.Properties;
using Service.RequestAndResponse.Request.Province;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Request.Street;
using Service.RequestAndResponse.Request.Ward;
using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.BookingOfServicesDetails;
using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.Districts;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.Locations;
using Service.RequestAndResponse.Response.Properties;
using Service.RequestAndResponse.Response.Provinces;
using Service.RequestAndResponse.Response.Services;
using Service.RequestAndResponse.Response.Streets;
using Service.RequestAndResponse.Response.Wards;
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

            CreateMap<HomeStayTypes, GetAllHomeStayType>();
            CreateMap<CreateHomeStayTypeRequest, HomeStayTypes>().ReverseMap();

            CreateMap<Property, GetAllProperties>();
            CreateMap<Property, CreatePropertyRequest>().ReverseMap();

            CreateMap<Services, GetAllServices>();
            CreateMap<Services, CreateServices>().ReverseMap();

            CreateMap<BookingDetail, GetBookingDetails>();
            CreateMap<BookingServices, GetAllBookingServices>();
            CreateMap<Booking, GetAllBookings>();
            CreateMap<BookingServicesDetail, GetAllDetailOfServices>();


            CreateMap<Location, GetLocation>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Ward))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Province));

            CreateMap<Location, GetAllLocation>();
            CreateMap<UpdateLocationRequest, Location>().ReverseMap();

            CreateMap<Province, GetAllProvince>();
            CreateMap<Province, GetProvince>();
            CreateMap<AddProvinceRequest, Province>().ReverseMap();
            CreateMap<UpdateProvinceRequest, Province>().ReverseMap();

            CreateMap<District, GetAllDistrict>();
            CreateMap<District, GetDistrict>();
            CreateMap<AddDistrictRequest, District>().ReverseMap();
            CreateMap<UpdateDistrictRequest, District>().ReverseMap();

            CreateMap<Ward, GetAllWard>();
            CreateMap<Ward, GetWard>();
            CreateMap<AddWardRequest, Ward>().ReverseMap();
            CreateMap<UpdateWardRequest, Ward>().ReverseMap();

            CreateMap<Street, GetAllStreet>();
            CreateMap<Street, GetStreet>();
            CreateMap<AddStreetRequest, Street>().ReverseMap();
            CreateMap<UpdateStreetRequest, Street>().ReverseMap();




        }
    }
}
