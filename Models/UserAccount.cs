namespace RunTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;
public class UserAccount {
    public int Id {get; set;}
    public string? Username {get; set;}
    public string? Password {get; set;}
    public string? Role {get; set;}
}