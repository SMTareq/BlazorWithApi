using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorWithApi.Shared.Models;

public class MealScheduleDto
{
    public int ScheduleId { get; set; }
    
    [Required(ErrorMessage = "Employee is required")]
    public int EmployeeId { get; set; }
    
    public string? EmployeeName { get; set; }
    
    [Required(ErrorMessage = "Meal date is required")]
    [DataType(DataType.Date)]
    public DateTime? MealDate { get; set; } = DateTime.Today;
    
    [StringLength(250)]
    public string? Notes { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // For dropdown binding
    public List<Employee>? AvailableEmployees { get; set; }
}
