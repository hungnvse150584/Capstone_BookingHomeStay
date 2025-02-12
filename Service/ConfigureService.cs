using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.Mapping;
using Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class ConfigureService
    {
        public static IServiceCollection ConfigureServiceService(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IHomeStayService, HomeStayService>();
            services.AddScoped<IImageServicesService, ImageServicesService>();
            services.AddScoped<IImageHomeStayTypesService, ImageHomeStayTypesService>();
            services.AddScoped<IHomeStayTypeService, HomeStayTypeService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IServiceServices, ServicesService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IWardService, WardService>();
            services.AddScoped<IStreetService, StreetService>();

            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
