using cs_apiEcommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace cs_apiEcommerce.Data;

public static class DataSeeder
{
    public static void SeedData(ApplicationDbContext appContext)
    {
        // Seeding de Roles
        if (!appContext.Roles.Any())
        {
            appContext.Roles.AddRange(
              new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
              new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
            );
        }
        // Seeding de Categorías
        if (!appContext.Categories.Any())
        {
            appContext.Categories.AddRange(
              new Category { Name = "Clothing and Accessories", CreationDate = DateTime.Now },
              new Category { Name = "Electronics", CreationDate = DateTime.Now },
              new Category { Name = "Sports", CreationDate = DateTime.Now },
              new Category { Name = "Home", CreationDate = DateTime.Now },
              new Category { Name = "Books", CreationDate = DateTime.Now }
            );
        }
        // Seeding de Usuario Administrador
        if (!appContext.ApplicationUsers.Any())
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = "admin-001",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                Name = "Administrador"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

            var regularUser = new ApplicationUser
            {
                Id = "user-001",
                UserName = "user@user.com",
                NormalizedUserName = "USER@USER.COM",
                Email = "user@user.com",
                NormalizedEmail = "USER@USER.COM",
                EmailConfirmed = true,
                Name = "Usuario Regular"
            };
            regularUser.PasswordHash = hasher.HashPassword(regularUser, "User123!");

            appContext.ApplicationUsers.AddRange(adminUser, regularUser);
        }
        // Seeding de UserRoles
        if (!appContext.UserRoles.Any())
        {
            appContext.UserRoles.AddRange(
              new IdentityUserRole<string> { UserId = "admin-001", RoleId = "1" }, // Admin
              new IdentityUserRole<string> { UserId = "user-001", RoleId = "2" }   // User
            );
        }

        // Seeding de Productos
        if (!appContext.Products.Any())
        {
            appContext.Products.AddRange(
              new Product
              {
                  Name = "Basic T-Shirt",
                  Description = "100% cotton t-shirt",
                  Price = 25.99m,
                  SKU = "PROD-001-TSH-M",
                  Stock = 50,
                  CategoryId = 1,
                  Category = appContext.Categories.Find(1)!,
                  ImgUrl = "https://via.placeholder.com/300x300/FF0000/FFFFFF?text=T-Shirt",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Galaxy Smartphone",
                  Description = "Smartphone with 128GB",
                  Price = 599.99m,
                  SKU = "PROD-002-PHO-BLK",
                  Stock = 25,
                  CategoryId = 2,
                  Category = appContext.Categories.Find(2)!,
                  ImgUrl = "https://via.placeholder.com/300x300/0000FF/FFFFFF?text=Smartphone",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Soccer Ball",
                  Description = "Official FIFA ball",
                  Price = 45.00m,
                  SKU = "PROD-003-BAL-WHT",
                  Stock = 30,
                  CategoryId = 3,
                  Category = appContext.Categories.Find(3)!,
                  ImgUrl = "https://via.placeholder.com/300x300/00FF00/FFFFFF?text=Ball",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Table Lamp",
                  Description = "Dimmable LED lamp",
                  Price = 89.99m,
                  SKU = "PROD-004-LAM-WHT",
                  Stock = 15,
                  CategoryId = 4,
                  Category = appContext.Categories.Find(4)!,
                  ImgUrl = "https://via.placeholder.com/300x300/FFFF00/000000?text=Lamp",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Don Quixote",
                  Description = "Classic novel by Cervantes",
                  Price = 19.99m,
                  SKU = "PROD-005-BOOK-ESP",
                  Stock = 100,
                  CategoryId = 5,
                  Category = appContext.Categories.Find(5)!,
                  ImgUrl = "https://via.placeholder.com/300x300/800080/FFFFFF?text=Book",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Classic Jeans",
                  Description = "Blue denim jeans",
                  Price = 79.99m,
                  SKU = "PROD-006-JNS-BLU",
                  Stock = 40,
                  CategoryId = 1,
                  Category = appContext.Categories.Find(1)!,
                  ImgUrl = "https://via.placeholder.com/300x300/4169E1/FFFFFF?text=Jeans",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Pro Tablet",
                  Description = "10.5 inch tablet with stylus included",
                  Price = 459.99m,
                  SKU = "PROD-007-TAB-SIL",
                  Stock = 20,
                  CategoryId = 2,
                  Category = appContext.Categories.Find(2)!,
                  ImgUrl = "https://via.placeholder.com/300x300/C0C0C0/000000?text=Tablet",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Running Shoes",
                  Description = "Sports shoes for running",
                  Price = 129.99m,
                  SKU = "PROD-008-SHO-BLK",
                  Stock = 35,
                  CategoryId = 3,
                  Category = appContext.Categories.Find(3)!,
                  ImgUrl = "https://via.placeholder.com/300x300/000000/FFFFFF?text=Shoes",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Express Coffee Maker",
                  Description = "Automatic coffee maker with built-in grinder",
                  Price = 299.99m,
                  SKU = "PROD-009-CFM-BLK",
                  Stock = 12,
                  CategoryId = 4,
                  Category = appContext.Categories.Find(4)!,
                  ImgUrl = "https://via.placeholder.com/300x300/2F4F4F/FFFFFF?text=Coffee+Maker",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Programming in C#",
                  Description = "Complete guide to programming in C# and .NET",
                  Price = 49.99m,
                  SKU = "PROD-010-BOOK-ENG",
                  Stock = 80,
                  CategoryId = 5,
                  Category = appContext.Categories.Find(5)!,
                  ImgUrl = "https://via.placeholder.com/300x300/008B8B/FFFFFF?text=C%23+Book",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Sports Jacket",
                  Description = "Waterproof jacket for outdoor activities",
                  Price = 149.99m,
                  SKU = "PROD-011-JKT-NAV",
                  Stock = 28,
                  CategoryId = 1,
                  Category = appContext.Categories.Find(1)!,
                  ImgUrl = "https://via.placeholder.com/300x300/000080/FFFFFF?text=Jacket",
                  CreationDate = DateTime.Now
              },
              new Product
              {
                  Name = "Bluetooth Headphones",
                  Description = "Wireless headphones with noise cancellation",
                  Price = 189.99m,
                  SKU = "PROD-012-HDP-BLK",
                  Stock = 45,
                  CategoryId = 2,
                  Category = appContext.Categories.Find(2)!,
                  ImgUrl = "https://via.placeholder.com/300x300/1C1C1C/FFFFFF?text=Headphones",
                  CreationDate = DateTime.Now
              }
            );
        }
        appContext.SaveChanges();
    }
}
