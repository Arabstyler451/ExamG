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


            //Producer User 4
            var producerUser4 = await userManager.FindByEmailAsync("producer4@test.com");
            if (producerUser4 == null)
            {
                producerUser4 = new IdentityUser { UserName = "producer4@test.com", Email = "producer4@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(producerUser4, "Producer123!");
            }

            if (!await userManager.IsInRoleAsync(producerUser4, "Producer"))
            {
                await userManager.AddToRoleAsync(producerUser4, "Producer");
            }


            //Producer User 5
            var producerUser5 = await userManager.FindByEmailAsync("producer5@test.com");
            if (producerUser5 == null)
            {
                producerUser5 = new IdentityUser { UserName = "producer5@test.com", Email = "producer5@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(producerUser5, "Producer123!");
            }

            if (!await userManager.IsInRoleAsync(producerUser5, "Producer"))
            {
                await userManager.AddToRoleAsync(producerUser5, "Producer");
            }


            //Producer User 6
            var producerUser6 = await userManager.FindByEmailAsync("producer6@test.com");
            if (producerUser6 == null)
            {
                producerUser6 = new IdentityUser { UserName = "producer6@test.com", Email = "producer6@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(producerUser6, "Producer123!");
            }

            if (!await userManager.IsInRoleAsync(producerUser6, "Producer"))
            {
                await userManager.AddToRoleAsync(producerUser6, "Producer");
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


            //Normal User 1
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


            //Normal User 2
            var customer2 = await userManager.FindByEmailAsync("user2@test.com");
            if (customer2 == null)
            {
                customer2 = new IdentityUser { UserName = "user2@test.com", Email = "user2@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(customer2, "User123!");
            }

            if (!await userManager.IsInRoleAsync(customer2, "User"))
            {
                await userManager.AddToRoleAsync(customer2, "User");
            }


            //Normal User 3
            var customer3 = await userManager.FindByEmailAsync("user3@test.com");
            if (customer3 == null)
            {
                customer3 = new IdentityUser { UserName = "user3@test.com", Email = "user3@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(customer3, "User123!");
            }

            if (!await userManager.IsInRoleAsync(customer3, "User"))
            {
                await userManager.AddToRoleAsync(customer3, "User");
            }


            //Normal User 4
            var customer4 = await userManager.FindByEmailAsync("user4@test.com");
            if (customer4 == null)
            {
                customer4 = new IdentityUser { UserName = "user4@test.com", Email = "user4@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(customer4, "User123!");
            }

            if (!await userManager.IsInRoleAsync(customer4, "User"))
            {
                await userManager.AddToRoleAsync(customer4, "User");
            }


            //Normal User 5
            var customer5 = await userManager.FindByEmailAsync("user5@test.com");
            if (customer5 == null)
            {
                customer5 = new IdentityUser { UserName = "user5@test.com", Email = "user5@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(customer5, "User123!");
            }

            if (!await userManager.IsInRoleAsync(customer5, "User"))
            {
                await userManager.AddToRoleAsync(customer5, "User");
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
            var producerUser4 = await userManager.FindByEmailAsync("producer4@test.com");
            var producerUser5 = await userManager.FindByEmailAsync("producer5@test.com");
            var producerUser6 = await userManager.FindByEmailAsync("producer6@test.com");


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
                    producerImage = "/images/producers/greenacreFarm.jpg",
                    UserId = producerUser.Id
                },

                new producers
                {
                    producerName = "Hillcrest Dairy",
                    producerEmail = "hillcrestdairy@example.com",
                    producerPhone = "07111222333",
                    producerDescription = "Small-scale herd of 40 Friesian cows, all grass-fed. Handmade cheeses aged on-site. Unpasteurised milk also available.",
                    producerLocation = "Midshire, SK50",
                    producerImage = "/images/producers/hillcrestDairy.jpg",
                    UserId = producerUser2.Id
                },
                
                new producers
                {
                    producerName = "The Old Mill Bakery",
                    producerEmail = "oldmillbakery@example.com",
                    producerPhone = "07555123456",
                    producerDescription = "Using locally-milled stoneground flour and slow fermentation. No additives, no preservatives - just honest bread.",
                    producerLocation = "Midshire, SK24",
                    producerImage = "/images/producers/theOldMillBakery.jpg",
                    UserId = producerUser3.Id
                },

                new producers
                {
                    producerName = "Moorside Butchery",
                    producerEmail = "moorsidebutchery@example.com",
                    producerPhone = "07411223344",
                    producerDescription = "Traditional dry-ageing and butchery. All animals reared within 15 miles, slaughtered locally. Full traceability guaranteed.",
                    producerLocation = "Midshire, SK42",
                    producerImage = "/images/producers/moorsideButchery.jpg",
                    UserId = producerUser4.Id
                },

                new producers
                {
                    producerName = "Meadow Apiary",
                    producerEmail = "meadowapiary@example.com",
                    producerPhone = "07899887766",
                    producerDescription = "200 hives across wildflower meadows and woodland. Raw, cold-extracted honey. A proportion of profits supports pollinator conservation.",
                    producerLocation = "Midshire, SK35",
                    producerImage = "/images/producers/meadowApiary.jpg",
                    UserId = producerUser5.Id
                },

                new producers
                {
                    producerName = "Sunfield Farm",
                    producerEmail = "sunfieldfarm@example.com",
                    producerPhone = "07222334455",
                    producerDescription = "Mixed family farm nestled in the sunny slopes of Midshire. We grow a wide range of fruit and vegetables, keep free-range hens for eggs, and rear rare-breed pigs. Everything is grown and raised with care using regenerative farming practices.",
                    producerLocation = "Midshire, SK39",
                    producerImage = "/images/producers/sunfieldFarm.jpg",
                    UserId = producerUser6.Id
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
                new categories { categoryName = "Fruit & Veg" },
                new categories { categoryName = "Dairy & Eggs" },
                new categories { categoryName = "Bakery" },
                new categories { categoryName = "Meat & Poultry" },
                new categories { categoryName = "Honey & Preserves" }
            };

            await context.categories.AddRangeAsync(list);
            await context.SaveChangesAsync();
        }


        public static async Task seedProducts(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure categories exist and retrieve them
            var vegCat = await context.categories.FirstOrDefaultAsync(c => c.categoryName == "Fruit & Veg");
            var dairyCat = await context.categories.FirstOrDefaultAsync(c => c.categoryName == "Dairy & Eggs");
            var bakeryCat = await context.categories.FirstOrDefaultAsync(c => c.categoryName == "Bakery");
            var meatCat = await context.categories.FirstOrDefaultAsync(c => c.categoryName == "Meat & Poultry");
            var honeyCat = await context.categories.FirstOrDefaultAsync(c => c.categoryName == "Honey & Preserves");

            if (vegCat == null || dairyCat == null || bakeryCat == null || meatCat == null || honeyCat == null)
                throw new Exception("Required categories not found.");

            // Finding the producers to associate products with
            var greenAcreFarm = await context.producers.FirstOrDefaultAsync(p => p.producerName == "Greenacre Farm");
            var hillCrestDairy = await context.producers.FirstOrDefaultAsync(p => p.producerName == "Hillcrest Dairy");
            var theOldMillBakery = await context.producers.FirstOrDefaultAsync(p => p.producerName == "The Old Mill Bakery");
            var moorsideButchery = await context.producers.FirstOrDefaultAsync(p => p.producerName == "Moorside Butchery");
            var meadowApiary = await context.producers.FirstOrDefaultAsync(p => p.producerName == "Meadow Apiary");
            var sunfieldFarm = await context.producers.FirstOrDefaultAsync(p => p.producerName == "Sunfield Farm");

            if (greenAcreFarm == null || hillCrestDairy == null || theOldMillBakery == null || moorsideButchery == null || meadowApiary == null || sunfieldFarm == null)
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
                        productUnit = "kg",
                        productAvailability = true,
                        productImage = "/images/products/heritageTomatoes.jpg",
                        producersId = greenAcreFarm.producersId,
                        categoriesId = vegCat.categoriesId
                    },

                    new products
                    {
                        productName = "Heritage Carrots",
                        productDescription = "Sweet, colorful carrots",
                        productPrice = 2.40f,
                        stockQuantity = 80,
                        productUnit = "bunch",
                        productAvailability = true,
                        productImage = "/images/products/heritageCarrots.jpg",
                        producersId = greenAcreFarm.producersId,
                        categoriesId = vegCat.categoriesId

                    },

                    new products
                    {
                        productName = "Raw Milk Cheddar",
                        productDescription = "Aged 12 months, crumbly",
                        productPrice = 5.50f,
                        stockQuantity = 0,
                        productUnit = "200g",
                        productAvailability = false,
                        productImage = "/images/products/rawMilkCheddar.jpg",
                        producersId = hillCrestDairy.producersId,
                        categoriesId = dairyCat.categoriesId
                    },

                    new products
                    {
                        productName = "Country Sourdough",
                        productDescription = "Long-fermented, stone-baked",
                        productPrice = 4.80f,
                        stockQuantity = 50,
                        productUnit = "loaf",
                        productAvailability = true,
                        productImage = "/images/products/countrySourdough.jpg",
                        producersId = theOldMillBakery.producersId,
                        categoriesId = bakeryCat.categoriesId
                    },

                    new products
                    {
                        productName = "Cox Apples",
                        productDescription = "Sweet, aromatic English eating apples with a lovely honeyed flavour",
                        productPrice = 3.50f,
                        stockQuantity = 120,
                        productUnit = "kg",
                        productAvailability = true,
                        productImage = "/images/products/coxApples.jpg",
                        producersId = sunfieldFarm.producersId,
                        categoriesId = vegCat.categoriesId
                    },
                    
                    new products
                    {
                        productName = "Mixed Salad Leaves",
                        productDescription = "Freshly picked mix of lettuce, rocket, spinach and baby chard",
                        productPrice = 2.80f,
                        stockQuantity = 60,
                        productUnit = "bag",
                        productAvailability = true,
                        productImage = "/images/products/mixedSaladLeaves.jpg",
                        producersId = greenAcreFarm.producersId,
                        categoriesId = vegCat.categoriesId
                    },
                    
                    new products
                    {
                        productName = "Grass-Fed Beef Mince",
                        productDescription = "20% fat, rich flavour from our grass-fed heritage breed cattle",
                        productPrice = 12.95f,
                        stockQuantity = 45,
                        productUnit = "kg",
                        productAvailability = true,
                        productImage = "/images/products/grassFedBeefMince.jpg",
                        producersId = moorsideButchery.producersId,
                        categoriesId = meatCat.categoriesId
                    },
                    
                    new products
                    {
                        productName = "Strawberry Preserve",
                        productDescription = "Handmade with ripe strawberries and unrefined cane sugar. Perfect on fresh bread",
                        productPrice = 4.25f,
                        stockQuantity = 35,
                        productUnit = "jar",
                        productAvailability = true,
                        productImage = "/images/products/strawberryPreserve.jpg",
                        producersId = sunfieldFarm.producersId,
                        categoriesId = honeyCat.categoriesId
                    },
                    
                    new products
                    {
                        productName = "Wildflower Honey",
                        productDescription = "Raw, unfiltered honey from our wildflower meadows. Rich, floral and full of goodness",
                        productPrice = 6.50f,
                        stockQuantity = 28,
                        productUnit = "350g",
                        productAvailability = true,
                        productImage = "/images/products/wildflowerHoney.jpg",
                        producersId = meadowApiary.producersId,
                        categoriesId = honeyCat.categoriesId
                    },
                    
                    new products
                    {
                        productName = "Free Range Eggs",
                        productDescription = "Large, golden-yolked eggs from our happy free-range hens",
                        productPrice = 3.75f,
                        stockQuantity = 150,
                        productUnit = "dozen",
                        productAvailability = true,
                        productImage = "/images/products/freeRangeEggs.jpg",
                        producersId = sunfieldFarm.producersId,
                        categoriesId = dairyCat.categoriesId
                    },

                    new products
                    {
                        productName = "Heirloom Potatoes",
                        productDescription = "A mix of waxy and floury varieties, perfect for roasting, mashing or salads",
                        productPrice = 2.90f,
                        stockQuantity = 70,
                        productUnit = "kg",
                        productAvailability = true,
                        productImage = "/images/products/heirloomPotatoes.jpg",
                        producersId = greenAcreFarm.producersId,
                        categoriesId = vegCat.categoriesId
                    },

                    new products
                    {
                        productName = "Free Range Chicken",
                        productDescription = "Whole birds reared on pasture with plenty of space to roam. Rich flavour and tender meat.",
                        productPrice = 14.95f,
                        stockQuantity = 9,
                        productUnit = "whole",
                        productAvailability = true,
                        productImage = "/images/products/freeRangeChicken.jpg",
                        producersId = moorsideButchery.producersId,
                        categoriesId = meatCat.categoriesId
                    },

                    new products
                    {
                        productName = "Sourdough Baguette",
                        productDescription = "Crisp crust and chewy crumb, perfect for sandwiches or as an accompaniment to meals",
                        productPrice = 2.50f,
                        stockQuantity = 40,
                        productUnit = "loaf",
                        productAvailability = true,
                        productImage = "/images/products/sourdoughBaguette.jpg",
                        producersId = theOldMillBakery.producersId,
                        categoriesId = bakeryCat.categoriesId
                    },

                    new products
                    {
                        productName = "Raw Milk Yogurt",
                        productDescription = "Creamy, tangy yogurt made from our unpasteurised milk. Packed with natural probiotics.",
                        productPrice = 4.00f,
                        stockQuantity = 25,
                        productUnit = "500g",
                        productAvailability = true,
                        productImage = "/images/products/rawMilkYogurt.jpg",
                        producersId = hillCrestDairy.producersId,
                        categoriesId = dairyCat.categoriesId
                    },

                    new products
                    {
                        productName = "Beef Sausages",
                        productDescription = "Handmade sausages, seasoned with herbs and spices. Perfect for grilling or frying.",
                        productPrice = 8.95f,
                        stockQuantity = 50,
                        productUnit = "pack of 6",
                        productAvailability = true,
                        productImage = "/images/products/beefSausages.jpg",
                        producersId = moorsideButchery.producersId,
                        categoriesId = meatCat.categoriesId
                    },

                    new products
                    {
                        productName = "Purple Sprouting Broccoli",
                        productDescription = "Tender stems and sweet florets - the best of the spring brassicas",
                        productPrice = 3.10f,
                        stockQuantity = 40,
                        productUnit = "bag",
                        productAvailability = true,
                        productImage = "/images/products/purpleSproutingBroccoli.jpg",
                        producersId = greenAcreFarm.producersId,
                        categoriesId = vegCat.categoriesId
                    }
                };
                await context.products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }



        // Seeding addresses to users so that seeding orders is possible
        public static async Task seedAddresses(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Check if addresses already exist
            if (await context.address.AnyAsync())
                return;

            // Get your users
            var normalUser1 = await userManager.FindByEmailAsync("user@test.com");
            var normalUser2 = await userManager.FindByEmailAsync("user2@test.com");
            var normalUser3 = await userManager.FindByEmailAsync("user3@test.com");
            var normalUser4 = await userManager.FindByEmailAsync("user4@test.com");
            var normalUser5 = await userManager.FindByEmailAsync("user5@test.com");

            var addresses = new List<address>
    {
        // User 1 addresses
        new address
        {
            UserId = normalUser1.Id,
            street = "123 Main Street",
            city = "Midshire",
            postalCode = "SK48 1AB",
            country = "England",
            IsDefault = true,
            createdDate = DateTime.Now.AddDays(-30)
        },
        new address
        {
            UserId = normalUser1.Id,
            street = "45 Oak Avenue",
            city = "Midshire",
            postalCode = "SK48 2CD",
            country = "England",
            IsDefault = false,
            createdDate = DateTime.Now.AddDays(-25)
        },
        
        // User 2 addresses
        new address
        {
            UserId = normalUser2.Id,
            street = "78 Pine Road",
            city = "Midshire",
            postalCode = "SK50 3EF",
            country = "England",
            IsDefault = true,
            createdDate = DateTime.Now.AddDays(-20)
        },
        
        // User 3 addresses
        new address
        {
            UserId = normalUser3.Id,
            street = "12 Beech Lane",
            city = "Midshire",
            postalCode = "SK39 4GH",
            country = "England",
            IsDefault = true,
            createdDate = DateTime.Now.AddDays(-15)
        },
        new address
        {
            UserId = normalUser3.Id,
            street = "99 Willow Drive",
            city = "Midshire",
            postalCode = "SK39 5IJ",
            country = "England",
            IsDefault = false,
            createdDate = DateTime.Now.AddDays(-10)
        },
        
        // User 4 addresses
        new address
        {
            UserId = normalUser4.Id,
            street = "7 Chestnut Close",
            city = "Midshire",
            postalCode = "SK42 6KL",
            country = "England",
            IsDefault = true,
            createdDate = DateTime.Now.AddDays(-12)
        },
        
        // User 5 addresses
        new address
        {
            UserId = normalUser5.Id,
            street = "33 Sycamore Avenue",
            city = "Midshire",
            postalCode = "SK35 7MN",
            country = "England",
            IsDefault = true,
            createdDate = DateTime.Now.AddDays(-8)
        }
    };

            await context.address.AddRangeAsync(addresses);
            await context.SaveChangesAsync();
        }



        public static async Task seedOrders(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Check if orders already exist
            if (await context.orders.AnyAsync())
                return;

            // Get your normal users
            var normalUser1 = await userManager.FindByEmailAsync("user@test.com");
            var normalUser2 = await userManager.FindByEmailAsync("user2@test.com");
            var normalUser3 = await userManager.FindByEmailAsync("user3@test.com");
            var normalUser4 = await userManager.FindByEmailAsync("user4@test.com");
            var normalUser5 = await userManager.FindByEmailAsync("user5@test.com");

            // Get default addresses for users (already seeded)
            var address1 = await context.address.FirstOrDefaultAsync(a => a.UserId == normalUser1.Id && a.IsDefault);
            var address2 = await context.address.FirstOrDefaultAsync(a => a.UserId == normalUser2.Id && a.IsDefault);
            var address3 = await context.address.FirstOrDefaultAsync(a => a.UserId == normalUser3.Id && a.IsDefault);
            var address4 = await context.address.FirstOrDefaultAsync(a => a.UserId == normalUser4.Id && a.IsDefault);
            var address5 = await context.address.FirstOrDefaultAsync(a => a.UserId == normalUser5.Id && a.IsDefault);

            // Get all products
            var allProducts = await context.products.ToListAsync();

            if (allProducts.Count == 0)
                throw new Exception("No products found to create orders.");

            var orders = new List<orders>();
            var orderProductsList = new List<orderProducts>();

            // ORDER 1: DELIVERY - Standard Delivery - User 1
            var heritageTomatoes = allProducts.First(p => p.productName == "Heritage Tomatoes");
            var coxApples = allProducts.First(p => p.productName == "Cox Apples");
            var wildflowerHoney = allProducts.First(p => p.productName == "Wildflower Honey");

            var order1 = new orders
            {
                UserId = normalUser1.Id,
                addressId = address1?.addressId, // Now safely exists from seedAddresses
                totalAmount = (heritageTomatoes.productPrice * 2) + (coxApples.productPrice * 1) + (wildflowerHoney.productPrice * 2) + 3.99f,
                delivery = true,
                collection = false,
                deliveryType = "Standard Delivery",
                deliveryFee = 3.99f,
                orderStatus = "Completed",
                orderCollectionDate = null,
                orderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                DeliveryStreet = address1?.street,
                DeliveryCity = address1?.city,
                DeliveryPostalCode = address1?.postalCode,
                DeliveryCountry = address1?.country
            };
            orders.Add(order1);

            orderProductsList.Add(new orderProducts
            {
                orders = order1,
                productsId = heritageTomatoes.productsId,
                quantity = 2,
                unitPrice = heritageTomatoes.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order1,
                productsId = coxApples.productsId,
                quantity = 1,
                unitPrice = coxApples.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order1,
                productsId = wildflowerHoney.productsId,
                quantity = 2,
                unitPrice = wildflowerHoney.productPrice
            });

            // ORDER 2: COLLECTION - User 2
            var rawMilkYogurt = allProducts.First(p => p.productName == "Raw Milk Yogurt");
            var sourdoughBaguette = allProducts.First(p => p.productName == "Sourdough Baguette");
            var beefSausages = allProducts.First(p => p.productName == "Beef Sausages");

            var order2 = new orders
            {
                UserId = normalUser2.Id,
                addressId = null, // No address for collection
                totalAmount = (rawMilkYogurt.productPrice * 2) + (sourdoughBaguette.productPrice * 2) + (beefSausages.productPrice * 1),
                delivery = false,
                collection = true,
                deliveryType = null,
                deliveryFee = 0f,
                orderStatus = "Shipped",
                orderCollectionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
                orderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-8)),
                DeliveryStreet = null,
                DeliveryCity = null,
                DeliveryPostalCode = null,
                DeliveryCountry = null
            };
            orders.Add(order2);

            orderProductsList.Add(new orderProducts
            {
                orders = order2,
                productsId = rawMilkYogurt.productsId,
                quantity = 2,
                unitPrice = rawMilkYogurt.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order2,
                productsId = sourdoughBaguette.productsId,
                quantity = 2,
                unitPrice = sourdoughBaguette.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order2,
                productsId = beefSausages.productsId,
                quantity = 1,
                unitPrice = beefSausages.productPrice
            });

            // ORDER 3: DELIVERY - First Class Delivery - User 3
            var freeRangeEggs = allProducts.First(p => p.productName == "Free Range Eggs");
            var countrySourdough = allProducts.First(p => p.productName == "Country Sourdough");
            var grassFedBeefMince = allProducts.First(p => p.productName == "Grass-Fed Beef Mince");
            var strawberryPreserve = allProducts.First(p => p.productName == "Strawberry Preserve");
            var mixedSaladLeaves = allProducts.First(p => p.productName == "Mixed Salad Leaves");

            var order3 = new orders
            {
                UserId = normalUser3.Id,
                addressId = address3.addressId,
                totalAmount = (freeRangeEggs.productPrice * 1) + (countrySourdough.productPrice * 1) +
                             (grassFedBeefMince.productPrice * 2) + (strawberryPreserve.productPrice * 2) +
                             (mixedSaladLeaves.productPrice * 1) + 5.99f, // + delivery fee
                delivery = true,
                collection = false,
                deliveryType = "First Class Delivery",
                deliveryFee = 5.99f,
                orderStatus = "Processing",
                orderCollectionDate = null,
                orderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
                DeliveryStreet = address3.street,
                DeliveryCity = address3.city,
                DeliveryPostalCode = address3.postalCode,
                DeliveryCountry = address3.country
            };
            orders.Add(order3);

            orderProductsList.Add(new orderProducts
            {
                orders = order3,
                productsId = freeRangeEggs.productsId,
                quantity = 1,
                unitPrice = freeRangeEggs.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order3,
                productsId = countrySourdough.productsId,
                quantity = 1,
                unitPrice = countrySourdough.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order3,
                productsId = grassFedBeefMince.productsId,
                quantity = 2,
                unitPrice = grassFedBeefMince.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order3,
                productsId = strawberryPreserve.productsId,
                quantity = 2,
                unitPrice = strawberryPreserve.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order3,
                productsId = mixedSaladLeaves.productsId,
                quantity = 1,
                unitPrice = mixedSaladLeaves.productPrice
            });

            // ORDER 4: COLLECTION - User 4
            var freeRangeChicken = allProducts.First(p => p.productName == "Free Range Chicken");
            var heirloomPotatoes = allProducts.First(p => p.productName == "Heirloom Potatoes");
            var heritageCarrots = allProducts.First(p => p.productName == "Heritage Carrots");

            var order4 = new orders
            {
                UserId = normalUser4.Id,
                addressId = null,
                totalAmount = (freeRangeChicken.productPrice * 1) + (heirloomPotatoes.productPrice * 3) + (heritageCarrots.productPrice * 1),
                delivery = false,
                collection = true,
                deliveryType = null,
                deliveryFee = 0f,
                orderStatus = "Pending",
                orderCollectionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)), // Future collection
                orderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                DeliveryStreet = null,
                DeliveryCity = null,
                DeliveryPostalCode = null,
                DeliveryCountry = null
            };
            orders.Add(order4);

            orderProductsList.Add(new orderProducts
            {
                orders = order4,
                productsId = freeRangeChicken.productsId,
                quantity = 1,
                unitPrice = freeRangeChicken.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order4,
                productsId = heirloomPotatoes.productsId,
                quantity = 3,
                unitPrice = heirloomPotatoes.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order4,
                productsId = heritageCarrots.productsId,
                quantity = 1,
                unitPrice = heritageCarrots.productPrice
            });

            // ORDER 5: DELIVERY - Next Day Delivery - User 5
            var rawMilkCheddar = allProducts.First(p => p.productName == "Raw Milk Cheddar");
            var purpleSproutingBroccoli = allProducts.First(p => p.productName == "Purple Sprouting Broccoli");

            var order5 = new orders
            {
                UserId = normalUser5.Id,
                addressId = address5.addressId,
                totalAmount = (rawMilkCheddar.productPrice * 3) + (sourdoughBaguette.productPrice * 2) + (purpleSproutingBroccoli.productPrice * 2) + 7.99f,
                delivery = true,
                collection = false,
                deliveryType = "Next Day Delivery",
                deliveryFee = 7.99f,
                orderStatus = "Delivered",
                orderCollectionDate = null,
                orderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                DeliveryStreet = address5.street,
                DeliveryCity = address5.city,
                DeliveryPostalCode = address5.postalCode,
                DeliveryCountry = address5.country
            };
            orders.Add(order5);

            orderProductsList.Add(new orderProducts
            {
                orders = order5,
                productsId = rawMilkCheddar.productsId,
                quantity = 3,
                unitPrice = rawMilkCheddar.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order5,
                productsId = sourdoughBaguette.productsId,
                quantity = 2,
                unitPrice = sourdoughBaguette.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order5,
                productsId = purpleSproutingBroccoli.productsId,
                quantity = 2,
                unitPrice = purpleSproutingBroccoli.productPrice
            });

            // ORDER 6: DELIVERY - Standard Delivery - User 1 (Second order, Cancelled)
            var beefSausages2 = allProducts.First(p => p.productName == "Beef Sausages");
            var mixedSaladLeaves2 = allProducts.First(p => p.productName == "Mixed Salad Leaves");
            var wildflowerHoney2 = allProducts.First(p => p.productName == "Wildflower Honey");

            var order6 = new orders
            {
                UserId = normalUser1.Id,
                addressId = address1.addressId,
                totalAmount = (beefSausages2.productPrice * 2) + (mixedSaladLeaves2.productPrice * 2) + (wildflowerHoney2.productPrice * 1) + 3.99f,
                delivery = true,
                collection = false,
                deliveryType = "Standard Delivery",
                deliveryFee = 3.99f,
                orderStatus = "Cancelled",
                orderCollectionDate = null,
                orderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-15)),
                DeliveryStreet = address1.street,
                DeliveryCity = address1.city,
                DeliveryPostalCode = address1.postalCode,
                DeliveryCountry = address1.country
            };
            orders.Add(order6);

            orderProductsList.Add(new orderProducts
            {
                orders = order6,
                productsId = beefSausages2.productsId,
                quantity = 2,
                unitPrice = beefSausages2.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order6,
                productsId = mixedSaladLeaves2.productsId,
                quantity = 2,
                unitPrice = mixedSaladLeaves2.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order6,
                productsId = wildflowerHoney2.productsId,
                quantity = 1,
                unitPrice = wildflowerHoney2.productPrice
            });

            // ORDER 7: COLLECTION - User 3 (Second order)
            var freeRangeChicken2 = allProducts.First(p => p.productName == "Free Range Chicken");
            var coxApples2 = allProducts.First(p => p.productName == "Cox Apples");
            var strawberryPreserve2 = allProducts.First(p => p.productName == "Strawberry Preserve");
            var rawMilkYogurt2 = allProducts.First(p => p.productName == "Raw Milk Yogurt");

            var order7 = new orders
            {
                UserId = normalUser3.Id,
                addressId = null,
                totalAmount = (freeRangeChicken2.productPrice * 1) + (coxApples2.productPrice * 2) +
                             (strawberryPreserve2.productPrice * 3) + (rawMilkYogurt2.productPrice * 2),
                delivery = false,
                collection = true,
                deliveryType = null,
                deliveryFee = 0f,
                orderStatus = "Processing",
                orderCollectionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)), // Future collection
                orderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                DeliveryStreet = null,
                DeliveryCity = null,
                DeliveryPostalCode = null,
                DeliveryCountry = null
            };
            orders.Add(order7);

            orderProductsList.Add(new orderProducts
            {
                orders = order7,
                productsId = freeRangeChicken2.productsId,
                quantity = 1,
                unitPrice = freeRangeChicken2.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order7,
                productsId = coxApples2.productsId,
                quantity = 2,
                unitPrice = coxApples2.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order7,
                productsId = strawberryPreserve2.productsId,
                quantity = 3,
                unitPrice = strawberryPreserve2.productPrice
            });

            orderProductsList.Add(new orderProducts
            {
                orders = order7,
                productsId = rawMilkYogurt2.productsId,
                quantity = 2,
                unitPrice = rawMilkYogurt2.productPrice
            });

            // Save all orders and order products
            await context.orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
            await context.orderProducts.AddRangeAsync(orderProductsList);
            await context.SaveChangesAsync();
        }



        public static async Task seedLoyaltyAccounts(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Skip if already seeded
            if (await context.loyaltyAccount.AnyAsync()) return;

            var users = new[]
            {
        "user@test.com", "user2@test.com", "user3@test.com",
        "user4@test.com", "user5@test.com"
    };

            foreach (var email in users)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null) continue;

                // Load all orders for this user
                var userOrders = await context.orders
                    .Where(o => o.UserId == user.Id)
                    .ToListAsync();

                if (!userOrders.Any()) continue;

                // Calculate points earned: 10 points per £1 spent (matches my ordersController logic)
                int totalPoints = userOrders
                    .Where(o => o.orderStatus != "Cancelled")
                    .Sum(o => (int)(o.totalAmount * 10));

                string tier = totalPoints switch
                {
                    >= 5000 => "Platinum",
                    >= 2000 => "Gold",
                    >= 500 => "Silver",
                    _ => "Bronze"
                };

                var loyaltyAccount = new loyaltyAccount
                {
                    UserId = user.Id,
                    pointsBalance = totalPoints,
                    loyaltyTier = tier,
                    redeemedOffers = string.Empty,
                    ActiveOffers = string.Empty,
                    ConsumedOffers = string.Empty,
                    PendingOffer = null
                };

                context.loyaltyAccount.Add(loyaltyAccount);
                await context.SaveChangesAsync();

                // Create an Earn transaction per non-cancelled order
                foreach (var order in userOrders.Where(o => o.orderStatus != "Cancelled"))
                {
                    int pointsEarned = (int)(order.totalAmount * 10);

                    context.loyaltyTransaction.Add(new loyaltyTransaction
                    {
                        loyaltyAccountId = loyaltyAccount.loyaltyAccountId,
                        ordersId = order.ordersId,
                        loyaltyPoints = pointsEarned,
                        transactionType = "Earn",
                        transactionDate = order.orderDate.ToDateTime(TimeOnly.MinValue)
                    });
                }

                await context.SaveChangesAsync();
            }
        }

    }
}
