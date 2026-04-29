using System.Text;
using DeliverySystem.Api.Application.Mappings;
using DeliverySystem.Api.Application.Services;
using DeliverySystem.Api.Application.Validators;
using DeliverySystem.Api.External.Clients;
using FluentValidation;
using FluentValidation.AspNetCore;
using Refit;
using DeliverySystem.Api.Domain.Interfaces;
using DeliverySystem.Api.Infrastructure.Repositories;
using DeliverySystem.Api.Infrastructure.Settings;
using DeliverySystem.Api.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Settings
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// MongoDB
var mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"]!;
var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"]!;

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CepService>();
builder.Services.AddScoped<DeliveryService>();
builder.Services.AddScoped<NotificationService>();

// Refit — ViaCEP
builder.Services.AddRefitClient<ICepClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://viacep.com.br"));

// JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:Secret"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// Controllers + Swagger com JWT
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// Verify MongoDB connection on startup
var db = app.Services.GetRequiredService<IMongoDatabase>();
await db.RunCommandAsync<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("ping", 1));
app.Logger.LogInformation("MongoDB connected: {DatabaseName}", mongoDatabaseName);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
