using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentGrievancePortal.Models;

namespace StudentGrievancePortal.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Grievance>Grievances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.ERP_Id)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

           
            modelBuilder.Entity<Grievance>()
                .Property(g => g.TicketNumber)
                .HasComputedColumnSql("'GRV-' + CAST([GrievanceId] AS NVARCHAR(10))");

            modelBuilder.Entity<Grievance>()
                .Property(g => g.Status)
                .HasDefaultValue("Submitted");

            modelBuilder.Entity<Grievance>()
                .Property(g => g.Priority)
                .HasDefaultValue("Medium");

            modelBuilder.Entity<Grievance>()
                .Property(g => g.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Grievance>()
                .Property(g => g.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>()
                .HasData(
                new User { UserId = 1, ERP_Id = "STU001", FullName = "Student1", Email = "student@college.edu", PasswordHash = "123", RoleId = 1, DeptId = 1 },
                new User { UserId = 2, ERP_Id = "CC001", FullName = "Prof. 1", Email = "coordinator@college.edu", PasswordHash = "123", RoleId = 2, DeptId = 1 }
                );

            modelBuilder.Entity<Role>()
                .HasData(
                new Role { RoleId = 1, RoleName = "Student" },
                new Role { RoleId = 2, RoleName = "Coordinator" },
                new Role { RoleId = 3, RoleName = "Admin" }
                );

            modelBuilder.Entity<Department>()
                 .HasData(
                new Department { DeptId = 1, DeptName = "MCA"},
                new Department { DeptId = 2, DeptName = "BA(JMC)"}                
                );
        }
    }
}
