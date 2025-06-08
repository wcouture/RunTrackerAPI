using Microsoft.EntityFrameworkCore;
using RunTrackerAPI.Models;

namespace RunTrackerAPI.Services;

/// <summary>
/// Service class for managing Account entities in the database.
/// Provides CRUD operations and additional functionality for Account records.
/// Implements the IAccountService interface for dependency injection and testing.
/// </summary>
public class AccountService : IAccountService
{
    private RunDb _db {get; set;} = null!;
    private IPasswordHasher _passwordHasher {get; set;} = null!;

    /// <summary>
    /// Initializes a new instance of the AccountService class.
    /// </summary>
    /// <param name="db">The database context for Account entities.</param>
    /// <param name="passwordHasher">The password hasher service for secure password handling.</param>
    public AccountService(RunDb db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Retrieves all user accounts from the database.
    /// </summary>
    /// <returns>A Task containing a List of UserAccount records.</returns>
    public async Task<List<UserAccount>> GetAllAccounts()
    {
        return await _db.AccountData.ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific user account by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user account to retrieve.</param>
    /// <returns>A Task containing an IResult with the following possible outcomes:
    /// - Results.Ok(account) if the account is found
    /// - Results.NotFound() if the account does not exist
    /// </returns>
    public async Task<IResult> Authenticate(UserAccount user)
    {
        var account = await _db.AccountData.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (account is null) return Results.NotFound();
        if (_passwordHasher.VerifyPassword(user.Password!, account.Password!))
        {
            return Results.Ok(account);
        }
        return Results.Unauthorized();
    }

    /// <summary>
    /// Creates a new user account in the database with a hashed password.
    /// </summary>
    /// <param name="account">The UserAccount object to be added to the database.</param>
    /// <returns>A Task containing an IResult with the following possible outcomes:
    /// - Results.Created() with the new account if successful
    /// - Results.BadRequest() if the username already exists
    /// </returns>
    /// <remarks>
    /// This method checks for existing usernames and hashes the password before storing it.
    /// </remarks>
    public async Task<IResult> CreateAccount(UserAccount account)
    {
        var existingAccount = await _db.AccountData.FirstOrDefaultAsync(u => u.Username == account.Username);
        if (existingAccount is not null) return Results.BadRequest("Username already exists");

        var hashedPassword = _passwordHasher.HashPassword(account.Password!);
        account.Password = hashedPassword;
        await _db.AccountData.AddAsync(account);
        await _db.SaveChangesAsync();
        return Results.Created($"/users/{account.Id}", account);
    }

    /// <summary>
    /// Updates an existing user account with new data.
    /// </summary>
    /// <param name="account">The updated UserAccount object containing the new data.</param>
    /// <param name="id">The unique identifier of the user account to update.</param>
    /// <returns>A Task containing an IResult with the following possible outcomes:
    /// - Results.NoContent() if the update is successful
    /// - Results.NotFound() if the account does not exist
    /// </returns>
    /// <remarks>
    /// This method hashes the new password before updating the account.
    /// </remarks>
    public async Task<IResult> UpdateAccount(UserAccount account, int id)
    {
        var existingAccount = await _db.AccountData.FindAsync(id);
        if (existingAccount is null) return Results.NotFound();

        existingAccount.Email = account.Email;
        existingAccount.Username = account.Username;
        existingAccount.Password = _passwordHasher.HashPassword(account.Password!);
        existingAccount.Role = account.Role;
        
        await _db.SaveChangesAsync();
        return Results.NoContent();
    }

    /// <summary>
    /// Deletes a specific user account from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the user account to delete.</param>
    /// <returns>A Task containing an IResult with the following possible outcomes:
    /// - Results.Ok() if the deletion is successful
    /// - Results.NotFound() if the account does not exist
    /// </returns>
    public async Task<IResult> DeleteAccount(int id)
    {
        var account = await _db.AccountData.FindAsync(id);
        if (account is null) return Results.NotFound();

        _db.AccountData.Remove(account);
        await _db.SaveChangesAsync();
        return Results.Ok();
    }
}