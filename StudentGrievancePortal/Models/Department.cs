using System.ComponentModel.DataAnnotations;

namespace StudentGrievancePortal.Models;

public class Department
{
    [Key]
    public int DeptId { get; set; }
    public string DeptName { get; set; } = string.Empty;
}