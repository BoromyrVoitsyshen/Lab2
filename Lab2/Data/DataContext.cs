using Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Ware> Wares { get; set; }
        public DbSet<WareOwner> WareOwners { get; set; }
        public DbSet<WareCategory> WareCategories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WareCategory>()
                .HasKey(pc => new { pc.WareId, pc.CategoryId });
            modelBuilder.Entity<WareCategory>()
                .HasOne(p => p.Ware)
                .WithMany(pc => pc.WareCategories)
                .HasForeignKey(p => p.WareId);
            modelBuilder.Entity<WareCategory>()
                .HasOne(c => c.Category)
                .WithMany(pc => pc.WareCategories)
                .HasForeignKey(c => c.CategoryId);

            modelBuilder.Entity<WareOwner>()
                .HasKey(po => new { po.WareId, po.OwnerId });
            modelBuilder.Entity<WareOwner>()
                .HasOne(p => p.Ware)
                .WithMany(po => po.WareOwners)
                .HasForeignKey(p => p.WareId);
            modelBuilder.Entity<WareOwner>()
                .HasOne(o => o.Owner)
                .WithMany(po => po.WareOwners)
                .HasForeignKey(o => o.OwnerId);

        }
    }
}
