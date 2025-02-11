using DataAccessObject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.BaseRepository;
using Repository.IBaseRepository;
using Repository.IRepositories;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public static class ConfigureService
    {
        public static IServiceCollection ConfigureRepositoryService(this IServiceCollection services, IConfiguration configuration)
        {
            //
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();

            services.AddScoped<IHomeStayRepository, HomeStayRepository>();
            services.AddScoped<IHomeStayTypeRepository, HomeStayTypeRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingServiceRepository, BookingServiceRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();


            services.AddScoped<INotificationRepository, NotificationRepository>();
            
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddTransient<IReviewRepository, ReviewRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddTransient<IReportRepository, ReportRepository>();

            services.AddTransient < ILocationRepository, LocationRepository>();
            services.AddTransient<IStreetRepository, StreetRepository>();
            services.AddTransient<IWardRepository, WardRepository>();
            services.AddTransient<IDistrictRepository, DistrictRepository>();
            services.AddTransient<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IImageServicesRepository, ImageServicesRepository>();
            services.AddScoped<IImageHomeStayTypesRepository, ImageHomeStayTypesRepository>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            //
            services.AddScoped<AccountDAO>();

            services.AddScoped<HomeStayDAO>();
            services.AddScoped<HomeStayTypeDAO>();
            services.AddScoped<PropertyDAO>();

            services.AddScoped<BookingDAO>();
            services.AddScoped<BookingServicesDAO>();
            services.AddScoped<ReviewDAO>();
            services.AddScoped<NotificationDAO>();
            services.AddScoped<ServicesDAO>();
            services.AddScoped<RatingDAO>();
            services.AddScoped<ImageHomeStayTypesDAO>();
            services.AddScoped<ReportDAO>();
            services.AddScoped<ImageServicesDAO>();
            services.AddScoped<LocationDAO>();
            services.AddScoped<StreetDAO>();
            services.AddScoped<WardDAO>();
            services.AddScoped<DistrictDAO>();
            services.AddScoped<ProvinceDAO>();

            //

            return services;
        }
    }
}
