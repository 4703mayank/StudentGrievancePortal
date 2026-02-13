using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentGrievancePortal.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    public string ERP_Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public int RoleId { get; set; }
    [ForeignKey("RoleId")]
    public Role? Role { get; set; }

    public int DeptId { get; set; }
    [ForeignKey("DeptId")]
    public Department? Department { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

}