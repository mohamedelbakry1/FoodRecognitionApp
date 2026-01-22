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
            if (_context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Any())
            {
                await _context.Database.MigrateAsync();
            }

            if (!_context.Categories.Any())
            {
                var categoriesdata = await File.ReadAllTextAsync(@"..\Infrastructure\FoodRecognitionApp.Persistence\Data\DataSeeding\category.json");

                var categories = JsonSerializer.Deserialize<List<Category>>(categoriesdata);

                if(categories is not null && categories.Count > 0)
                {
                    await _context.Categories.AddRangeAsync(categories);
                }
            }

            if (!_context.Foods.Any())
            {
                var foodsdata = await File.ReadAllTextAsync(@"..\Infrastructure\FoodRecognitionApp.Persistence\Data\DataSeeding\food.json");

                var foods = JsonSerializer.Deserialize<List<Food>>(foodsdata);

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
    }
}
