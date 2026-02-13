using System.ComponentModel.DataAnnotations;

namespace StudentGrievancePortal.Models;

public class Role
{
    [Key]
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}