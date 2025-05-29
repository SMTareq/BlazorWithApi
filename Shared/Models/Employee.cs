using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorWithApi.Shared.Models;

public class Employee
{
    public Employee()
    {
        MealSchedules = new HashSet<MealSchedule>();
    }

    [Key]
    public int Id { get; set; }
    
    [Required]
    public Guid EmployeeId { get; set; } = Guid.NewGuid();
    
    [StringLength(250)]
    public string? EmployeeName { get; set; }
    
    [StringLength(50)]
    public string? EmployeeCode { get; set; }
    
    public bool? IsActive { get; set; }
    
    // Navigation property for MealSchedules
    [InverseProperty("Employee")]
    public virtual ICollection<MealSchedule> MealSchedules { get; set; }
}
