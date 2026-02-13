using Microsoft.EntityFrameworkCore;

namespace StudentGrievancePortal.Models;

public class GrievanceContext : DbContext
{
    public GrievanceContext(DbContextOptions<GrievanceContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Grievance> Grievances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grievance>()
            .Property(p => p.TicketNumber)
            .ValueGeneratedOnAddOrUpdate();
    }
}