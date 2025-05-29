using Microsoft.EntityFrameworkCore;
using BlazorWithApi.Shared.Models;

namespace BlazorWithApi.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<MealSchedule> MealSchedules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // This will be used if the context is created without configuration
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Disable automatic migrations
        modelBuilder.HasDefaultSchema("dbo");
        
        // Map to existing table
        modelBuilder.Entity<Employee>().ToTable("Employee");
        
        // Configure the Employee entity to match your existing table
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeId");
            entity.Property(e => e.EmployeeName).HasColumnName("EmployeeName").HasMaxLength(250);
            entity.Property(e => e.EmployeeCode).HasColumnName("EmployeeCode").HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("IsActive");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.ExpenseId).HasColumnName("ExpenseId").HasDefaultValueSql("NEWID()");
            entity.Property(e => e.ExpenseDate).HasColumnName("ExpenseDate");
            entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(255);
        });

        // Configure MealSchedule entity
        modelBuilder.Entity<MealSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId);
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleId");
            entity.Property(e => e.EmployeeId).IsRequired();
            entity.Property(e => e.MealDate).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            
            // Configure relationship with Employee
            entity.HasOne(ms => ms.Employee)
                .WithMany(e => e.MealSchedules)
                .HasForeignKey(ms => ms.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MealSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId);
            entity.ToTable("MealSchedule");
            
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleId");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeId");
            entity.Property(e => e.MealDate).HasColumnName("MealDate");
            entity.Property(e => e.Notes).HasColumnName("Notes").HasMaxLength(250);
            entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(false);
            
            // Configure the relationship with Employee
            entity.HasOne(ms => ms.Employee)
                  .WithMany()
                  .HasForeignKey(ms => ms.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
