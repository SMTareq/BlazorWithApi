using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorWithApi.Shared.Models;

public class Expense
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public Guid ExpenseId { get; set; } = Guid.NewGuid();
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;
    
    [StringLength(255)]
    public string? Description { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
}
