using GreenfieldLocalHubWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace GreenfieldLocalHubWebApp.Data
{
    public class SeedData
    {
        public static async Task seedRolesAndUsers(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //All the Seeded roles
            string[] roleNames = { "Admin", "Producer", "User", "Developer" };
            foreach (string roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);

                }
            }



            // Seeding my users and assigning them to the roles


            //Admin User
            var adminUser = await userManager.FindByEmailAsync("admin@test.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = "admin@test.com", Email = "admin@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Admin123!");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }



            //Producer User 1
            var producerUser = await userManager.FindByEmailAsync("producer@test.com");
            if (producerUser == null)
            {
                producerUser = new IdentityUser { UserName = "producer@test.com", Email = "producer@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(producerUser, "Producer123!");
            }

            if (!await userManager.IsInRoleAsync(producerUser, "Producer"))
            {
                await userManager.AddToRoleAsync(producerUser, "Producer");
            }



            //Producer User 2
            var producerUser2 = await userManager.FindByEmailAsync("producer2@test.com");
            if (producerUser2 == null)
            {
                producerUser2 = new IdentityUser { UserName = "producer2@test.com", Email = "producer2@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(producerUser2, "Producer123!");
            }

            if (!await userManager.IsInRoleAsync(producerUser2, "Producer"))
            {
                await userManager.AddToRoleAsync(producerUser2, "Producer");
            }


            //Producer User 3
            var producerUser3 = await userManager.FindByEmailAsync("producer3@test.com");
            if (producerUser3 == null)
            {
                producerUser3 = new IdentityUser { UserName = "producer3@test.com", Email = "producer3@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(producerUser3, "Producer123!");
            }

            if (!await userManager.IsInRoleAsync(producerUser3, "Producer"))
            {
                await userManager.AddToRoleAsync(producerUser3, "Producer");
            }



            //Developer User
            var developerUser = await userManager.FindByEmailAsync("dev@test.com");
            if (developerUser == null)
            {
                developerUser = new IdentityUser { UserName = "dev@test.com", Email = "dev@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(developerUser, "Dev123!");
            }

            if (!await userManager.IsInRoleAsync(developerUser, "Developer"))
            {
                await userManager.AddToRoleAsync(developerUser, "Developer");
            }


            //Normal User
            var customer = await userManager.FindByEmailAsync("user@test.com");
            if (customer == null)
            {
                customer = new IdentityUser { UserName = "user@test.com", Email = "user@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(customer, "User123!");
            }

            if (!await userManager.IsInRoleAsync(customer, "User"))
            {
                await userManager.AddToRoleAsync(customer, "User");
            }

        }


        // Seeding producers with their details (name, description, contact info, etc.) and associating them with the producer users created above.
        public static async Task seedProducers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Finding the producer users created in the previous method to associate them with producer details
            var producerUser = await userManager.FindByEmailAsync("producer@test.com");
            var producerUser2 = await userManager.FindByEmailAsync("producer2@test.com");
            var producerUser3 = await userManager.FindByEmailAsync("producer3@test.com");

            if (producerUser == null || producerUser2 == null || producerUser3 == null)
            {
                throw new Exception("Producer users not found.");
            }


            //preventing duplicate seeding of producers
            if (context.producers.Any())
                return; // Producers already seeded

            var producers = new List<producers>
            {
                new producers
                {
                    producerName = "Greenacre Farm",
                    producerEmail = "greenacrefarm@example.com",
                    producerPhone = "07123456789",
                    producerDescription = "Family-run for four generations. Organic certified with a focus on heirloom vegetable varieties and soil health regeneration.",
                    producerLocation = "Midshire, SK48",
                    producerImage = "/images/greenacreFarm.jpg",
                    UserId = producerUser.Id
                },

                new producers
                {
                    producerName = "Hillcrest Dairy",
                    producerEmail = "hillcrestdairy@example.com",
                    producerPhone = "07111222333",
                    producerDescription = "Small-scale herd of 40 Friesian cows, all grass-fed. Handmade cheeses aged on-site. Unpasteurised milk also available.",
                    producerLocation = "Midshire, SK50",
                    producerImage = "/images/hillcrestDairy.jpg",
                    UserId = producerUser2.Id
                },
                
                new producers
                {
                    producerName = "The Old Mill Bakery",
                    producerEmail = "oldmillbakery@example.com",
                    producerPhone = "07555123456",
                    producerDescription = "Using locally-milled stoneground flour and slow fermentation. No additives, no preservatives - just honest bread.",
                    producerLocation = "Midshire, SK24",
                    producerImage = "/images/theOldMillBakery.jpg",
                    UserId = producerUser3.Id
                }
            };

            context.producers.AddRange(producers);
            await context.SaveChangesAsync();
        }

        public static async Task seedCategories(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (await context.categories.AnyAsync()) return;

            var list = new List<categories>
            {
                new categories { categoryName = "Vegetables" },
                new categories { categoryName = "Dairy" },
                new categories { categoryName = "Bakery" }
            };

            await context.categories.AddRangeAsync(list);
            await context.SaveChangesAsync();
        }


        public static async Task seedProducts(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure categories exist and retrieve them
            var vegCat = await context.categories.FirstOrDefaultAsync(c => c.categoryName == "Vegetables");
            var dairyCat = await context.categories.FirstOrDefaultAsync(c => c.categoryName == "Dairy");
            var bakeryCat = await context.categories.FirstOrDefaultAsync(c => c.categoryName == "Bakery");
            if (vegCat == null || dairyCat == null || bakeryCat == null)
                throw new Exception("Required categories not found.");

            // Finding the producers to associate products with
            var greenAcreFarm = await context.producers.FirstOrDefaultAsync(p => p.producerName == "Greenacre Farm");
            var hillCrestDairy = await context.producers.FirstOrDefaultAsync(p => p.producerName == "Hillcrest Dairy");
            var theOldMillBakery = await context.producers.FirstOrDefaultAsync(p => p.producerName == "The Old Mill Bakery");

            if (greenAcreFarm == null || hillCrestDairy == null || theOldMillBakery == null)
            {
                throw new Exception("Producers not found.");
            }

            if (!context.products.Any())
            {
                var products = new List<products>
                {
                    new products
                    {
                        productName = "Heritage Tomatoes",
                        productDescription = "Mixed heritage varieties",
                        productPrice = 3.20f,
                        stockQuantity = 100,
                        productAvailability = true,
                        productImage = "/images/heritageTomatoes.jpg",
                        producersId = greenAcreFarm.producersId,
                        categoriesId = vegCat.categoriesId
                    },

                    new products
                    {
                        productName = "Heritage Carrots",
                        productDescription = "Sweet, colorful carrots",
                        productPrice = 2.40f,
                        stockQuantity = 80,
                        productAvailability = true,
                        productImage = "/images/heritageCarrots.jpg",
                        producersId = greenAcreFarm.producersId,
                        categoriesId = vegCat.categoriesId

                    },

                    new products
                    {
                        productName = "Raw Milk Cheddar",
                        productDescription = "Aged 12 months, crumbly",
                        productPrice = 5.50f,
                        stockQuantity = 0,
                        productAvailability = false,
                        productImage = "/images/rawMilkCheddar.jpg",
                        producersId = hillCrestDairy.producersId,
                        categoriesId = dairyCat.categoriesId
                    },

                    new products
                    {
                        productName = "Country Sourdough",
                        productDescription = "Long-fermented, stone-baked",
                        productPrice = 4.80f,
                        stockQuantity = 50,
                        productAvailability = true,
                        productImage = "/images/countrySourdough.jpg",
                        producersId = theOldMillBakery.producersId,
                        categoriesId = bakeryCat.categoriesId
                    }
                };
                await context.products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }

    }
}
