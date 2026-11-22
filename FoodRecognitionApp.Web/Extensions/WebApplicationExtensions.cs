using FoodRecognitionApp.Domain.Contracts;

namespace FoodRecognitionApp.Web.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> SeedAndMigrateDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbIntializer = scope.ServiceProvider.GetRequiredService<IDbIntializer>(); // Ask CLR to Create Object From IDbInitializer
            await dbIntializer.SeedDataAsync();

            return app;
        }
    }
}
