using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KhumaloCraft.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace KhumaloCraft.Data
{
    public class KhumaloCraftContext : IdentityDbContext
    {
        public KhumaloCraftContext (DbContextOptions<KhumaloCraftContext> options)
            : base(options)
        {
        }
             
        public DbSet<KhumaloCraft.Models.MyWorkModel> MyWorkModel { get; set; } = default!;
        public DbSet<KhumaloCraft.Models.CategoryModel> CategoryModel { get; set; } = default!;
        public DbSet<KhumaloCraft.Models.UserDetails> UserDetails { get; set; } = default!;
        public DbSet<KhumaloCraft.Models.Orders> Orders { get; set; } = default!;
        public DbSet<KhumaloCraft.Models.OrderItem> OrderItems { get; set; } = default!;
        public DbSet<KhumaloCraft.Models.Cart> Cart { get; set; } = default!;
        public DbSet<KhumaloCraft.Models.CartItems> CartItems { get; set; } = default!;

    }
}
