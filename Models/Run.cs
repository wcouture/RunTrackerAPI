using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RunTrackerAPI.Models;

public class Run
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public string? Label { get; set; }
    public Duration? Duration { get; set; }
    public double Mileage { get; set; }
}

public class Duration
{
    public int Id { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
}

public class RunDb : DbContext
{
    public RunDb(DbContextOptions options) : base(options) { }
    public DbSet<Run> RunData { get; set; } = null!;
    public DbSet<Duration> Durations { get; set; } = null!;
    public DbSet<UserAccount> AccountData { get; set; } = null!;
}