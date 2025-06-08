using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RunTrackerAPI.Models;

public class UserAccount {
    [Key]
    public int Id {get; set;}
    public string? Email {get; set;}
    public string? Username {get; set;}
    public string? Password {get; set;}
    public string? Role {get; set;}
}