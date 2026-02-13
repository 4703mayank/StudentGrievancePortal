using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentGrievancePortal.Models;

public class Grievance
{
    [Key]
    public int GrievanceId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string? TicketNumber { get; set; }

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "Submitted";
    public string Priority { get; set; } = "Medium";

    public int? StudentId { get; set; } 
    public User? Student {  get; set; }

    public int? AssignedDeptId { get; set; }
    [ForeignKey("AssignedDeptId")]
    public Department? Department { get; set; }

    public string? ResolutionDetails { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}