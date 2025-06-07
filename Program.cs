using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using RunTrackerAPI.Models;
using RunTrackerAPI.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RunData") ?? "Data Source=RunData.db";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<RunDb>(connectionString);
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RunTracker API",
        Description = "Track your running progress",
        Version = "v1"
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RunDb>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RunTracker API");
    });
}

app.MapGet("/", () => "Hello World!");

app.MapGet("/runs", async (RunDb db) =>  await db.RunData.Include(r => r.Duration).ToListAsync() );
app.MapPost("/run", async (RunDb db, Run run) =>
{
    await db.AddAsync(run);
    await db.SaveChangesAsync();
    return Results.Created($"/run/{run.Id}", run);
});

app.MapGet("/run/{id}", async (RunDb db, int id) => await db.RunData.Include(r => r.Duration).Where(r => r.Id == id).FirstAsync());

app.MapPut("/run/{id}", async (RunDb db, Run updatedRun, int id) =>
{
    var run = await db.RunData.FindAsync(id);
    if (run is null) return Results.NotFound();

    run.Label = updatedRun.Label;
    run.Duration = updatedRun.Duration;
    run.Mileage = updatedRun.Mileage;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/run/{id}", async (RunDb db, int id) =>
{
    var run = await db.RunData.FindAsync(id);
    if (run is null) return Results.NotFound();

    db.RunData.Remove(run);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/durs", async (RunDb db) => await db.Durations.ToListAsync());

app.MapPost("/user", async (RunDb db, UserAccount user, IPasswordHasher _passwordHasher) => {
    var existingUser = await db.AccountData.FirstOrDefaultAsync(u => u.Username == user.Username);
    bool isPasswordValid = _passwordHasher.VerifyPassword(user.Password!, existingUser!.Password!);
    if (existingUser is not null && isPasswordValid) return Results.Ok(existingUser);

    return Results.Unauthorized();
});
app.MapPost("/register", async (RunDb db, UserAccount user, IPasswordHasher _passwordHasher) =>
{
    var existingUser = await db.AccountData.FirstOrDefaultAsync(u => u.Username == user.Username);
    if (existingUser is not null) return Results.BadRequest("Username already exists");

    var hashedPassword = _passwordHasher.HashPassword(user.Password!);
    user.Password = hashedPassword;
    await db.AddAsync(user);
    await db.SaveChangesAsync();
    return Results.Created($"/user/{user.Id}", user);
});

app.Run();