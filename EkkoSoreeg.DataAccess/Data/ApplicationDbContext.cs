using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EkkoSoreeg.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace EkkoSoreeg.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options){ }
        public virtual DbSet<Catagory> TbCatagory { get; set; }
        public virtual DbSet<Product> TbProduct { get; set; }
        public virtual DbSet<ApplicationUser> TbapplicationUser { get; set; }
        public virtual DbSet<ShoppingCart> TbShoppingCarts { get; set; }
        public virtual DbSet<OrderHeader> TbOrderHeaders { get; set; }
        public virtual DbSet<OrderDetails> TbOrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
