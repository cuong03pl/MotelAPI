using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using Motel.Interfaces;
using Motel.Models;
using Motel.Repository;
using Motel.Services;
using Motel.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Motel.Hubs;
var builder = WebApplication.CreateBuilder(args);

// Cấu hình MongoDB cho Guid
BsonDefaults.GuidRepresentationMode = MongoDB.Bson.GuidRepresentationMode.V2;
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// Configure string serialization for GUIDs in DTOs
BsonSerializer.RegisterSerializer(typeof(string), new StringSerializer(BsonType.String));

builder.Configuration.GetSection("MotelDatabase").Bind(builder.Services);
// Cấu hình SignalR với các tùy chọn nâng cao
builder.Services.AddSignalR(options => {
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 102400; // 100 KB
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    options.KeepAliveInterval = TimeSpan.FromMinutes(1);
});

builder.Services.Configure<MotelDatabaseSettings>(builder.Configuration.GetSection("MotelDatabase"));
builder.Services.AddSingleton<MotelService>();

builder.Services.AddSingleton<IPostRepository, PostRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IReportRepository, ReportRepository>();
builder.Services.AddSingleton<IReviewRepository, ReviewRepository>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
builder.Services.AddSingleton<INewsRepository, NewsRepository>();
builder.Services.AddSingleton<IBookingRepository, BookingRepository>();
builder.Services.AddSingleton<ILoginHistoryRepository, LoginHistoryRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<RandomImage>();
builder.Services.AddSingleton<GenerateSlug>();

builder.Services.AddSingleton<IMessageRepository, MessageRepository>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
        builder.Configuration.GetValue<string>("MotelDatabase:ConnectionString"),
        builder.Configuration.GetValue<string>("MotelDatabase:DatabaseName"))
    .AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<ApplicationRole>>();

builder.Services.AddControllers();  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));

// Cấu hình CORS để cho phép kết nối từ client với credentials
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "https://motel-user-nine.vercel.app", "https://motel-admin-cyan.vercel.app", "http://localhost:3001")    // Địa chỉ client của bạn
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();                    
    });
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
    };
    
    // Cấu hình JWT cho SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // Nếu request đến endpoint của SignalR
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && 
                path.StartsWithSegments("/chatHub"))
            {
                // Đọc token từ query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Sử dụng chính sách CORS mới
app.UseCors("ClientPermission");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

// Map SignalR hub
app.MapHub<ChatHub>("/chatHub");

app.Run();
