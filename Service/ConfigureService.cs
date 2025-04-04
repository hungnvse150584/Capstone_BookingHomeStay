﻿using Microsoft.AspNetCore.Cors.Infrastructure;
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
            services.AddScoped<IHomeStayTypeService, HomeStayRentalService>();
            services.AddScoped<IRoomTypeService, RoomTypeService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<ICheckOutService, CheckOutService>();
            services.AddScoped<IBookingForService, BookingForService>();
            services.AddScoped<ICommissionRateService, CommissionRateService>();
            services.AddScoped<IImageHomeStayService, ImageHomeStayService>();
            services.AddScoped<ICancellationPolicyService, CancellationPolicyService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IServiceServices, ServicesService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IChatService, ChatService>();
            return services;
        }
    }
}
