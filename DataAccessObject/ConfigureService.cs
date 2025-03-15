using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using DataAccessObject.IBaseDAO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public static class ConfigureService
    {
        public static IServiceCollection ConfigureDataAccessObjectService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Account>();
            services.AddScoped<HomeStay>();
            services.AddScoped<HomeStayRentals>();
            services.AddScoped<RoomTypes>();
            services.AddScoped<Room>();
            services.AddScoped<Booking>();
            services.AddScoped<BookingServices>();
            services.AddScoped<BookingDetail>();
            services.AddScoped<BookingServicesDetail>();
            services.AddScoped<Services>();
            services.AddScoped<Report>();
            services.AddScoped<Notification>();
            services.AddScoped<Rating>();
            services.AddScoped<Review>();
            services.AddScoped<RefreshToken>();

            services.AddScoped(typeof(IBaseDAO<>), typeof(BaseDAO<>));
            return services;
        }
    }
}
