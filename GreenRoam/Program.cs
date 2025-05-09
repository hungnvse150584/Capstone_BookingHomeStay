//using BusinessObject.Model;
//using CloudinaryDotNet;
//using DataAccessObject;
//using GreenRoam.HangFireSetup;
//using GreenRoam.Hubs;
//using Hangfire;
//using Hangfire.SqlServer;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Any;
//using Microsoft.OpenApi.Models;
//using Newtonsoft.Json;
//using Repository;
//using Service;
//using Service.Hubs;
//using Service.IService;
//using Service.Service;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using Account = BusinessObject.Model.Account;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
//AppDomain.CurrentDomain.SetData("TimeZone", vietnamZone);
//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

////add Serializer
//builder.Services.AddControllers().AddNewtonsoftJson(options =>
//{
//    //options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
//    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Unspecified;
//    //options.SerializerSettings.Converters.Clear(); // Xóa các converter mặc định để tránh xung đột
//    //options.SerializerSettings.Converters.Add(new VietnamDateTimeConverter());
//    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
//    options.SerializerSettings.Converters.Clear(); // Xóa các converter mặc định
//    options.SerializerSettings.Converters.Add(new VietnamDateTimeConverter());
//    //options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
//    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
//    options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss";// Đảm bảo định dạng ISO

//});
//// Connect Database
//builder.Services.AddDbContext<GreenRoamContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"),
//        sqlOptions => sqlOptions.MigrationsAssembly("DataAccessObject"))
//    .AddInterceptors(new VietnamTimeInterceptor()));

//// Configure Services
//builder.Services.ConfigureRepositoryService(builder.Configuration);
//builder.Services.ConfigureServiceService(builder.Configuration);
//builder.Services.ConfigureDataAccessObjectService(builder.Configuration);


////Add SwaggerGen for Authentication
//builder.Services.AddSwaggerGen(option =>
//{
//    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
//    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter a valid token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    option.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type=ReferenceType.SecurityScheme,
//                    Id="Bearer"
//                }
//            },
//            new string[]{}
//        }
//    });
//    option.MapType<DateTime>(() => new OpenApiSchema
//    {
//        Type = "string",
//        Format = "date-time",
//        Example = new OpenApiString("2025-05-09T00:00:00")
//    });
//    // Cấu hình hỗ trợ file upload (multipart/form-data)
//    //option.MapType<IFormFile>(() => new OpenApiSchema
//    //{
//    //    Type = "string",
//    //    Format = "binary"
//    //});
//    //option.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
//    //{
//    //    Url = "https://hungnv.iselab.cloud:7221"
//    //});
//});

//// SetUp Specification for password
//builder.Services.AddIdentity<Account, IdentityRole>(options =>
//{
//    options.Password.RequireDigit = true;
//    options.Password.RequireLowercase = true;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireNonAlphanumeric = true;
//    options.Password.RequiredLength = 8;
//    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
//})
//.AddEntityFrameworkStores<GreenRoamContext>()
//.AddDefaultTokenProviders();

//builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
//{
//    options.TokenLifespan = TimeSpan.FromHours(1);
//});
////Add Authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme =
//    options.DefaultChallengeScheme =
//    options.DefaultForbidScheme =
//    options.DefaultScheme =
//    options.DefaultSignInScheme =
//    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidIssuer = builder.Configuration["JWT:Issuer"],
//        ValidateAudience = true,
//        ValidAudience = builder.Configuration["JWT:Audience"],
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(
//            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
//        )
//    };
//    options.Events = new JwtBearerEvents
//    {
//        OnMessageReceived = context =>
//        {
//            var accessToken = context.Request.Query["access_token"];
//            var path = context.HttpContext.Request.Path;
//            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
//            {
//                context.Token = accessToken;
//            }
//            return Task.CompletedTask;
//        }
//    };
//});

//builder.Services.AddHangfire(config => config
//    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
//    .UseSimpleAssemblyNameTypeSerializer()
//    .UseRecommendedSerializerSettings()
//    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DbConnection"), new SqlServerStorageOptions
//    {
//        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
//        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
//        QueuePollInterval = TimeSpan.Zero,
//        UseRecommendedIsolationLevel = true,
//        DisableGlobalLocks = true
//    }));

//builder.Services.AddHangfireServer();

//builder.Services.AddSignalR();
////builder.Services.AddCors(options =>
////{
////    options.AddDefaultPolicy(
////        builder => builder
////            .AllowAnyOrigin()
////            .AllowAnyHeader()
////            .AllowAnyMethod()
////            .AllowCredentials());
////});
////builder.Services.AddCors(options =>
////{
////    options.AddPolicy("AllowAll", builder =>
////    {
////        builder.WithOrigins("http://localhost:5173", "https://localhost:7221")
////               .AllowAnyMethod()
////               .AllowAnyHeader()
////               .AllowCredentials();
////    });
////});

//builder.Services.AddAuthorization();
//var cloudName = builder.Configuration["Cloudinary:CloudName"];
//var apiKey = builder.Configuration["Cloudinary:ApiKey"];
//var apiSecret = builder.Configuration["Cloudinary:ApiSecret"];
//var cloudinaryAccount = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
//var cloudinary = new Cloudinary(cloudinaryAccount);
//builder.Services.AddSingleton(cloudinary);
//var app = builder.Build();


//app.UseRouting();
//app.UseCors("AllowAll");
//// Configure the HTTP request pipeline.
//app.UseSwagger();
//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenRoam");
//    options.RoutePrefix = "swagger";
//});
//app.UseSwagger();
//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenRoam");
//    options.RoutePrefix = "swagger";
//});

//var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
//recurringJobManager.AddOrUpdate<IBookingService>(
//    "cancel-expired-bookings",
//    service => service.CancelExpiredBookings(),
//    Cron.MinuteInterval(5)); // Chạy job mỗi 5 phút

//recurringJobManager.AddOrUpdate<IBookingService>(
//    "check-out-bookings",
//    service => service.AutoCheckOutBookings(),
//    Cron.MinuteInterval(5)); // Chạy job mỗi 5 phút

///*app.UseHangfireDashboard("/hangfire"); */
////http://localhost:7221/hangfire

//app.UseHangfireDashboard("/hangfire", new DashboardOptions
//{
//    Authorization = new[] { new DashboardNoAuthFilter() }
//});

//app.UseHttpsRedirection();
////app.UseHangfireDashboard();
//app.UseStaticFiles();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();
//app.MapHangfireDashboard();

//app.MapHub<ChatHub>("/chatHub");
//app.MapHub<NotificationHub>("/notificationHub");
//app.MapHub<NotificationHubs>("/notificationHubs");
//app.MapFallbackToFile("/index.html");

//app.Run();
using BusinessObject.Model;
using CloudinaryDotNet;
using DataAccessObject;
using GreenRoam.HangFireSetup;
using GreenRoam.Hubs;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Repository;
using Service;
using Service.Hubs;
using Service.IService;
using Service.Service;
using Swashbuckle.AspNetCore.SwaggerGen;
using Account = BusinessObject.Model.Account;

var builder = WebApplication.CreateBuilder(args);

// Timezone setup
TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
AppDomain.CurrentDomain.SetData("TimeZone", vietnamZone);

// Danh sách origin được phép
var allowedOrigins = new[]
{
    "https://cho-tot-travel-managements.vercel.app", // Web FE
    "http://localhost:5173", // Dev FE
    "https://localhost:7221" // Dev BE
};

// Thêm CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClients", builder =>
    {
        builder.WithOrigins(allowedOrigins)
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
               .SetIsOriginAllowed(origin =>
               {
                   // Cho phép null (mobile) hoặc origin trong danh sách
                   return origin == null || allowedOrigins.Contains(origin);
               });
    });
});

// Connect Database
builder.Services.AddDbContext<GreenRoamContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("DataAccessObject"))
    .AddInterceptors(new VietnamTimeInterceptor()));

// Configure Services
builder.Services.ConfigureRepositoryService(builder.Configuration);
builder.Services.ConfigureServiceService(builder.Configuration);
builder.Services.ConfigureDataAccessObjectService(builder.Configuration);

// Add Controllers and JSON Serializer
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Unspecified;
    options.SerializerSettings.Converters.Clear();
    options.SerializerSettings.Converters.Add(new VietnamDateTimeConverter());
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
    options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss";
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
    option.MapType<DateTime>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date-time",
        Example = new OpenApiString("2025-05-10T00:00:00")
    });
});

// SetUp Specification for password
builder.Services.AddIdentity<Account, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
})
.AddEntityFrameworkStores<GreenRoamContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Add Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DbConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(25);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
});

builder.Services.AddAuthorization();

// Cloudinary
var cloudName = builder.Configuration["Cloudinary:CloudName"];
var apiKey = builder.Configuration["Cloudinary:ApiKey"];
var apiSecret = builder.Configuration["Cloudinary:ApiSecret"];
var cloudinaryAccount = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
var cloudinary = new Cloudinary(cloudinaryAccount);
builder.Services.AddSingleton(cloudinary);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.UseCors("AllowClients"); // Áp dụng CORS trước Authentication
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenRoam");
    options.RoutePrefix = "swagger";
});

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<IBookingService>(
    "cancel-expired-bookings",
    service => service.CancelExpiredBookings(),
    Cron.MinuteInterval(5));

recurringJobManager.AddOrUpdate<IBookingService>(
    "check-out-bookings",
    service => service.AutoCheckOutBookings(),
    Cron.MinuteInterval(5));

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new DashboardNoAuthFilter() }
});

app.MapControllers();
app.MapHangfireDashboard();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<NotificationHubs>("/notificationHubs");
app.MapGet("/api/health/hub", () => "Hub is running");
app.MapFallbackToFile("/index.html");

app.Run();