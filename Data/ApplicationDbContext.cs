using Microsoft.EntityFrameworkCore;
using WebPage.Models;
using WebPage.Data;

namespace WebPage.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }


    }
}
