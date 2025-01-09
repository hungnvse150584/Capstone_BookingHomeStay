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

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingServiceRepository, BookingServiceRepository>();

            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddScoped<IRatingRepository, RatingRepository>();
            
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddTransient<IReportRepository, ReportRepository>();

            services.AddTransient < ILocationRepository, LocationRepository>();
            services.AddTransient<IStreetRepository, StreetRepository>();
            services.AddTransient<IWardRepository, WardRepository>();
            services.AddTransient<IDistrictRepository, DistrictRepository>();
            services.AddTransient<IProvinceRepository, ProvinceRepository>();

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            //
            services.AddScoped<AccountDAO>();

            services.AddScoped<HomeStayDAO>();
            services.AddScoped<HomeStayTypeDAO>();

            services.AddScoped<BookingDAO>();
            services.AddScoped<BookingServicesDAO>();

            services.AddScoped<NotificationDAO>();

            services.AddScoped<RatingDAO>();
        
            services.AddScoped<ReportDAO>();

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
