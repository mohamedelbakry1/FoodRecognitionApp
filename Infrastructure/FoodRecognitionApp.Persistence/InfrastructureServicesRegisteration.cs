using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Persistence.AiModel;
using FoodRecognitionApp.Persistence.Data.Contexts;
using FoodRecognitionApp.Persistence.DataSeeding;
using FoodRecognitionApp.Persistence.Email;
using FoodRecognitionApp.Persistence.Identity.Entities;
using FoodRecognitionApp.Persistence.Identity.Services;
using FoodRecognitionApp.Persistence.Reports;
using FoodRecognitionApp.Persistence.Repositories;
using FoodRecognitionApp.Services.Abstraction.AIModel;
using FoodRecognitionApp.Services.Abstraction.AttachmentService;
using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Services.Abstraction.Email;
using FoodRecognitionApp.Services.Abstraction.Reports;
using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence
{
    public static class InfrastructureServicesRegisteration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IDbIntializer, CatalogDataSeeder>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAttachmentService, AttachmentService.AttachmentService>();
            services.AddScoped<IAIModelService, AIFoodRecogntionService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPdfReportGenerator, PdfReportGenerator>();

            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? throw new InvalidOperationException("Email Settings is missing");

            services.AddHttpClient<IAIModelService, AIFoodRecogntionService>((client =>
            {
                client.BaseAddress = new Uri(configuration["AiModelSettings:BaseUrl"]!);
                client.Timeout = TimeSpan.FromSeconds(
                    int.Parse(configuration["AiModelSettings:TimeoutSeconds"]!));
            }));


            services.AddIdentityCore<UserAccount>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddRoles<Role>()
              .AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? throw new InvalidOperationException("Jwt Settings is missing");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            return services;
        }
    }
}
