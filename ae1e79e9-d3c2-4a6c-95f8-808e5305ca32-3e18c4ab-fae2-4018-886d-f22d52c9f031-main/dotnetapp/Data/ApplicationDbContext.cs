using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using dotnetapp.Models;

namespace dotnetapp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>   // Extends IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Define DbSets for your entities
        public DbSet<CookingClass> CookingClasses { get; set; }
        public DbSet<CookingClassRequest> CookingClassRequests { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<CookingClassRequest>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.CookingClassRequests)
                .HasForeignKey(cr => cr.UserId);


            modelBuilder.Entity<CookingClassRequest>()
                .HasOne(cr => cr.CookingClass)
                .WithMany(c => c.CookingClassRequests)
                .HasForeignKey(cr => cr.CookingClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
