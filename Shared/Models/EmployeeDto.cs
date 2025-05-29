using System.ComponentModel.DataAnnotations;

namespace BlazorWithApi.Shared.Models;

public class EmployeeDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Employee ID is required")]
    public string EmployeeId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Employee name is required")]
    [StringLength(100, ErrorMessage = "Employee name cannot exceed 100 characters")]
    public string EmployeeName { get; set; } = string.Empty;
    
    [StringLength(50, ErrorMessage = "Employee code cannot exceed 50 characters")]
    public string? EmployeeCode { get; set; }
    
    public bool IsActive { get; set; } = true;
}
