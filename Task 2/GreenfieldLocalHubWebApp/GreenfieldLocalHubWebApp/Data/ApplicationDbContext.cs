using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GreenfieldLocalHubWebApp.Models;

namespace GreenfieldLocalHubWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GreenfieldLocalHubWebApp.Models.address> address { get; set; } = default!;
        public DbSet<GreenfieldLocalHubWebApp.Models.categories> categories { get; set; } = default!;
        public DbSet<GreenfieldLocalHubWebApp.Models.loyaltyAccount> loyaltyAccount { get; set; } = default!;
        public DbSet<GreenfieldLocalHubWebApp.Models.loyaltyTransaction> loyaltyTransaction { get; set; } = default!;
        public DbSet<GreenfieldLocalHubWebApp.Models.orderProducts> orderProducts { get; set; } = default!;
        public DbSet<GreenfieldLocalHubWebApp.Models.orders> orders { get; set; } = default!;
        public DbSet<GreenfieldLocalHubWebApp.Models.producers> producers { get; set; } = default!;
        public DbSet<GreenfieldLocalHubWebApp.Models.products> products { get; set; } = default!;
        public DbSet<GreenfieldLocalHubWebApp.Models.shoppingCart> shoppingCart { get; set; } = default!;
        public DbSet<GreenfieldLocalHubWebApp.Models.shoppingCartItems> shoppingCartItems { get; set; } = default!;
    }
}
