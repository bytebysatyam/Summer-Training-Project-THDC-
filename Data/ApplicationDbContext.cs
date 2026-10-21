using Microsoft.EntityFrameworkCore;
using WebPage.Models;

namespace WebPage.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<LeaveApplication> LeaveApplications => Set<LeaveApplication>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Complaint>().Property(c => c.Status).HasDefaultValue("Pending");
            modelBuilder.Entity<LeaveApplication>().Property(l => l.Status).HasDefaultValue("Pending");
        }
    }
}
