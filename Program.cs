using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using RunTrackerAPI.Models;
using RunTrackerAPI.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RunData") ?? "Data Source=RunData.db";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<RunDb>(connectionString);
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<RunService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "PaceMates API",
        Description = "Track and share your running progress",
        Version = "v2"
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaceMates API");
    });
}

app.MapGet("/", () => "Welcome to PaceMates!");

// Run CRUD
app.MapGet("/runs",             async (RunService runService) => await runService.GetAllRuns());
app.MapGet("/runs/{userId}",    async (RunService runService, int userId) => await runService.GetRunsByUserId(userId));
app.MapPost("/run",             async (RunService runService, Run run) => await runService.CreateRun(run));
app.MapGet("/run/{id}",         async (RunService runService, int id) => await runService.GetRunById(id));
app.MapPut("/run/{id}",         async (RunService runService, Run updatedRun, int id) => await runService.UpdateRun(updatedRun, id));
app.MapDelete("/run/{id}",      async (RunService runService, int id) => await runService.DeleteRun(id));

// User Authentication
app.MapGet("/users",            async (AccountService accountService) => await accountService.GetAllAccounts());
app.MapGet("/users/{id}",       async (AccountService accountService, int id) => await accountService.GetAccountById(id));
app.MapPost("/register",        async (AccountService accountService, UserAccount user) => await accountService.CreateAccount(user));
app.MapPut("/users/{id}",       async (AccountService accountService, UserAccount user, int id) => await accountService.UpdateAccount(user, id));
app.MapDelete("/users/{id}",    async (AccountService accountService, int id) => await accountService.DeleteAccount(id));

app.Run();