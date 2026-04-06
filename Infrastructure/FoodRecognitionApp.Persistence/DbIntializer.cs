using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Persistence.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FoodRecognitionApp.Persistence
{
    public class DbIntializer
        (
            AppDbContext _context,
            UserManager<UserAccount> _userManager,
            RoleManager<Role> _roleManager
        ) : IDbIntializer
    {
        public async Task IntializeAsync() 
        {
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await _context.Database.MigrateAsync();
            }

            if (!_context.Categories.Any())
            {
                var categories = await LoadDataFromJsonAsync<Category>("category.json");

                if(categories is not null && categories.Count > 0)
                {
                    await _context.Categories.AddRangeAsync(categories);
                }
            }

            if (!_context.Foods.Any())
            {
                var foods = await LoadDataFromJsonAsync<Food>("food.json");

                if(foods is not null && foods.Count > 0)
                {
                    await _context.Foods.AddRangeAsync(foods);
                }
            }

            if (!_context.Roles.Any())
            {
                await _roleManager.CreateAsync(new Role() { Name = "SuperAdmin" });
                await _roleManager.CreateAsync(new Role() { Name = "Admin" });
            }

            if (!_context.Users.Any())
            {
                var superAdmin = new UserAccount()
                {
                    UserName = "SuperAdmin",
                    Email = "SuperAdmin@gmail.com",
                    PhoneNumber = "01552013062"
                };

                var admin = new UserAccount()
                {
                    UserName = "Admin",
                    Email = "Admin@gmail.com",
                    PhoneNumber = "01015594223"
                };

                await _userManager.CreateAsync(superAdmin, "P@ssw0rd");
                await _userManager.CreateAsync(admin, "P@ssw0rd");

                await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                await _userManager.AddToRoleAsync(superAdmin, "Admin");
            }
            // ..\Infrastructure\FoodRecognitionApp.Persistence\Data\DataSeeding\food.json
            await _context.SaveChangesAsync();
        }

        private static async Task<List<T>> LoadDataFromJsonAsync<T>(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", fileName);
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Seeding file '{fileName}' was not found at: {filePath}");

            var Data = await File.ReadAllTextAsync(filePath);

            var Options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            return JsonSerializer.Deserialize<List<T>>(Data, Options) ?? new List<T>();
        }
    }
}
