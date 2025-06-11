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
builder.Services.AddScoped<InviteService>();

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
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "PaceMates API");
    });
}

app.MapGet("/", () => "Welcome to PaceMates!");

app.MapDelete("/delete/{secret}",        async (RunDb db, string secret) => {
    if (secret == "1234567890") {
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        await db.SaveChangesAsync();
        return Results.Ok("Database deleted and recreated");
    }
    return Results.BadRequest("Invalid secret");
});

// Run CRUD
app.MapGet(     "/runs/all",            async (RunService runService) => await runService.GetAllRuns());
app.MapGet(     "/runs/user/{userId}",  async (RunService runService, int userId) => await runService.GetRunsByUserId(userId));
app.MapPost(    "/run/create",          async (RunService runService, Run run) => await runService.CreateRun(run));
app.MapGet(     "/run/find/{id}",       async (RunService runService, int id) => await runService.GetRunById(id));
app.MapPut(     "/run/update/{id}",     async (RunService runService, Run updatedRun, int id) => await runService.UpdateRun(updatedRun, id));
app.MapDelete(  "/run/delete/{id}",     async (RunService runService, int id) => await runService.DeleteRun(id));

// User Authentication
app.MapGet(     "/user/all",                            async (AccountService accountService) => await accountService.GetAllAccounts());
app.MapGet(     "/user/find/{id}",                      async (AccountService accountService, int id) => await accountService.UserInfo(id));
app.MapGet(     "/user/search/{searchTerm}",            async (AccountService accountService, string searchTerm) => await accountService.SearchAccounts(searchTerm));
app.MapPost(    "/user/login",                          async (AccountService accountService, UserAccount user) => await accountService.Authenticate(user));
app.MapPost(    "/user/register",                       async (AccountService accountService, UserAccount user) => await accountService.CreateAccount(user)); // TODO: Add register
app.MapPut(     "/user/update/{id}",                    async (AccountService accountService, UserAccount user, int id) => await accountService.UpdateAccount(user, id)); // TODO: Add update
app.MapDelete(  "/user/delete/{id}",                    async (AccountService accountService, int id) => await accountService.DeleteAccount(id)); // TODO: Add delete
app.MapGet(     "/user/friends/all/{id}",               async (AccountService accountService, int id) => await accountService.GetAccountFriends(id));
app.MapDelete(  "/user/friends/remove/{id}/{friendId}", async (AccountService accountService, int id, int friendId) => await accountService.RemoveFriend(id, friendId)); // TODO: Add remove friend

// Friend Invites
app.MapPost(    "/invites/create",                   async (InviteService inviteService, FriendInvite newInvite) => await inviteService.CreateInvite(newInvite));
app.MapGet(     "/invites/user/{userId}",            async (InviteService inviteService, int userId) => await inviteService.GetInvitesByUserId(userId));
app.MapPut(     "/invites/accept/{receiverId}/{id}", async (InviteService inviteService, int receiverId, int id) => await inviteService.AcceptInvite(receiverId, id));
app.MapDelete(  "/invites/reject/{receiverId}/{id}", async (InviteService inviteService, int receiverId, int id) => await inviteService.RejectInvite(receiverId, id));
app.MapDelete(  "/invites/cancel/{id}",              async (InviteService inviteService, int senderId, int id) => await inviteService.DeleteInvite(senderId, id));

app.Run();