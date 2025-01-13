using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using Motel.Interfaces;
using Motel.Models;
using Motel.Repository;
using Motel.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.GetSection("MotelDatabase").Bind(builder.Services);

builder.Services.Configure<MotelDatabaseSettings>(builder.Configuration.GetSection("MotelDatabase"));
builder.Services.AddSingleton<MotelService>();
builder.Services.AddSingleton<IPostRepository, PostRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IReportRepository, ReportRepository>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
        builder.Configuration.GetValue<string>("MotelDatabase:ConnectionString"),
        builder.Configuration.GetValue<string>("MotelDatabase:DatabaseName"))
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
