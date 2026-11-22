using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Persistence;
using FoodRecognitionApp.Services;
using FoodRecognitionApp.Web.Extensions;
using FoodRecognitionApp.Web.Middlewares;
using Microsoft.Extensions.FileProviders;
using ScottPlot.Statistics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FoodRecognitionApp.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddApplicationPart(typeof(FoodRecognitionApp.Presentation.AuthController).Assembly)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMemoryCache();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });


            var app = builder.Build();

            await app.SeedAndMigrateDataAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseSwagger(); 
            app.UseSwaggerUI();

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
                RequestPath = "/Uploads"
            });
            app.UseMiddleware<GlobalErrorHandlingMiddlewares>();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
