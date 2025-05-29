using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorWithApi.Shared.Models;

public class MealSchedule
{
    [Key]
    public int ScheduleId { get; set; }
    
    [Required]
    public int EmployeeId { get; set; }
    
    [ForeignKey("EmployeeId")]
    public virtual Employee? Employee { get; set; }
    
    [Required(ErrorMessage = "Meal date is required")]
    [DataType(DataType.Date)]
    public DateTime? MealDate { get; set; } = DateTime.Today;
    
    [StringLength(250)]
    public string? Notes { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation property
    public virtual ICollection<Employee>? Employees { get; set; }
    
    public MealSchedule()
    {
        // Initialize navigation properties to prevent null reference exceptions
        Employee = new Employee();
    }
}
