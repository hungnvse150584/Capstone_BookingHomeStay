using BusinessObject.Model;
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
            services.AddScoped<ICommissionRateRepository, CommissionRateRepository>();
            services.AddScoped<IHomeStayRentalRepository, HomeStayRentalRepository>();
            services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IPricingRepository, PricingRepository>();
         

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingServiceRepository, BookingServiceRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IImageHomeStayRepository, ImageHomeStayRepository>();

            services.AddScoped<INotificationRepository, NotificationRepository>();
            
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddTransient<IReviewRepository, ReviewRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddTransient<IReportRepository, ReportRepository>();
            services.AddScoped<ICancellationPolicyRepository, CancellationPolicyRepository>();

            
            services.AddScoped<IImageServicesRepository, ImageServicesRepository>();
            services.AddScoped<IImageHomeStayTypesRepository, ImageHomeStayTypesRepository>();
            services.AddScoped<IImageRoomTypesRepository, ImageRoomTypesRepository>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
          
            
            services.AddScoped<AccountDAO>();
            services.AddScoped<ImageRoomTypeDAO>();
            services.AddScoped<HomeStayDAO>();
            services.AddScoped<CommissionRateDAO>();
            services.AddScoped<HomeStayRentalDAO>();
            services.AddScoped<RoomTypesDAO>();
            services.AddScoped<RoomDAO>();
            services.AddScoped<PricingDAO>();
            services.AddScoped<ImageHomeStayDAO>();
            services.AddScoped<CancellationPolicyDAO>();
            services.AddScoped<BookingDAO>();
            services.AddScoped<BookingServicesDAO>();
            services.AddScoped<ReviewDAO>();
            services.AddScoped<NotificationDAO>();
            services.AddScoped<ServicesDAO>();
            services.AddScoped<RatingDAO>();
            services.AddScoped<ImageHomeStayRentalsDAO>();
            services.AddScoped<ReportDAO>();
            services.AddScoped<ImageServicesDAO>();

            //

            return services;
        }
    }
}
