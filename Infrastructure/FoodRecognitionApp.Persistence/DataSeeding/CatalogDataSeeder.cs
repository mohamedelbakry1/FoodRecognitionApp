using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Persistence.Data.Contexts;
using FoodRecognitionApp.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FoodRecognitionApp.Persistence.DataSeeding
{
    internal class CatalogDataSeeder
        (
            AppDbContext _dbcontext,
            UserManager<UserAccount> _userManager,
            RoleManager<Role> _roleManager,
            ILogger<CatalogDataSeeder> _logger
        ) : IDbIntializer
    {
        public async Task SeedDataAsync(CancellationToken ct = default) 
        {
            if ((await _dbcontext.Database.GetPendingMigrationsAsync(ct)).Any())
                await _dbcontext.Database.MigrateAsync(ct);

            var seedRoot = Path.Combine(AppContext.BaseDirectory, "Data", "Files");

            await LoadDataFromJsonAsync<Category, int>(seedRoot, "category.json", ct);
            await LoadDataFromJsonAsync<Food, int>(seedRoot, "food.json", ct);

            if (!_dbcontext.Roles.Any())
            {
                await _roleManager.CreateAsync(new Role() { Name = "SuperAdmin" });
                await _roleManager.CreateAsync(new Role() { Name = "Admin" });
            }

            if (!_dbcontext.Users.Any())
            {
                var superAdmin = new UserAccount()
                {
                    UserName = "SuperAdmin",
                    DisplayName = "Super Admin",
                    Email = "SuperAdmin@gmail.com",
                    PhoneNumber = "01552013062"
                };

                var admin = new UserAccount()
                {
                    UserName = "Admin",
                    DisplayName = "Admin",
                    Email = "Admin@gmail.com",
                    PhoneNumber = "01015594223"
                };

                await _userManager.CreateAsync(superAdmin, "P@ssw0rd");
                await _userManager.CreateAsync(admin, "P@ssw0rd");

                await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                await _userManager.AddToRoleAsync(superAdmin, "Admin");
            }

            await _dbcontext.SaveChangesAsync(ct);
        }

        private async Task LoadDataFromJsonAsync<TEntity, TKey>(string roorPath, string fileName, CancellationToken ct = default) where TEntity : BaseEntity<TKey>
        {
            if (await _dbcontext.Set<TEntity>().AnyAsync(ct))
            {
                _logger.LogInformation("Table has already seeded");
                return;
            }

            var filePath = Path.Combine(roorPath, fileName);
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File {fileName} is not found!");
                return;
            }

            using var fileStream = File.OpenRead(filePath);

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            var items = await JsonSerializer.DeserializeAsync<List<TEntity>>(fileStream, options, ct);
            if (items?.Any() ?? false)
                _dbcontext.Set<TEntity>().AddRange(items);
        }
    }
}
